/* eslint-disable radix */
/* eslint-disable no-await-in-loop */
import React, { useCallback, useEffect, useMemo, useState } from 'react'
import { useHistory, useLocation, useParams } from 'react-router-dom/cjs/react-router-dom.min'
import EventTypes from 'src/business/eventTypes'
import { getGroupsOfFuelType } from 'src/business/fuelTypes'
import ModeState from 'src/business/modeState'
import Status from 'src/business/status'
import { ModalActions } from 'src/components/Modal/ModalProps'
import useModal from 'src/components/Modal/useModal'
import PastModalContainer from 'src/components/PasteModalContainer/PasteModalContainer'
import useAuth from 'src/context/useAuth'
import DateSelector from 'src/pages/Reports/components/DateSelector/DateSelector'
import TankSelector from 'src/pages/Reports/components/PerformanceContainer/BdnSelector/TankSelector'
import createDto from 'src/pages/Reports/utils/createDto'
import createView from 'src/pages/Reports/utils/createView'
import $, { ReportFields } from 'src/pages/Reports/utils/enums'
import { fetchCompleteBunkeringData } from 'src/pages/Reports/utils/fetchBunkeringCompleteData'
import {
  fetchProjectedBunkeringData,
  fillProjectedBunkeringData
} from 'src/pages/Reports/utils/fetchBunkeringPlanProjectedData'
import getFuelProps from 'src/pages/Reports/utils/getFuelProps'
import groupRename, { groupReset } from 'src/pages/Reports/utils/groupRename'
import isNullOrEmpty from 'src/pages/Reports/utils/isNullOrEmpty'
import margePerformanceFields from 'src/pages/Reports/utils/margePerformanceFields'
import mergeGroups from 'src/pages/Reports/utils/mergeGroups'
import createReportModel from 'src/pages/Reports/utils/modelCreators'
import performanceTabs, { performanceFields } from 'src/pages/Reports/utils/performanceTabs'
import processConsumptions from 'src/pages/Reports/utils/processConsumptions'
import processData from 'src/pages/Reports/utils/processData'
import processPerformanceData from 'src/pages/Reports/utils/processPerformanceData'
import seedData from 'src/pages/Reports/utils/seedData'
import http from 'src/services/http'
import reportState from 'src/services/storage/reportState'
import arraysAreIdentical from 'src/utils/arraysAreIdentical'
import formatDate from 'src/utils/formatDate'
import { truncateString2 } from 'src/utils/stringUtils'

export const consSuffix = ['_me', '_dg1', '_dg2', '_dg3', '_dg4', '_dge']

export const filterFuelType = key => b => !!b && b.fuelType === (key.includes('hfo') ? 1 : 2)

export const PAGE_SIZE = 10

export const groups = (requiresPlanFields, hidden) =>
  [
    !hidden && { key: 'robHfoActual', groupId: 1, label: 'ROB HFO ACTUAL' },
    { key: 'robHfoPool', groupId: 2, label: `ROB HFO${!hidden ? ' POOL' : ''}` },
    !hidden && { key: 'robMgoActual', groupId: 3, label: 'ROB MGO ACTUAL' },
    { key: 'robMgoPool', groupId: 4, label: `ROB MGO${!hidden ? ' POOL' : ''}` },
    requiresPlanFields && { key: 'planHfo', groupId: 7, label: 'HFO' },
    requiresPlanFields && { key: 'planMgo', groupId: 8, label: 'MGO' }
  ].filter(Boolean)

const ReportsContext = React.createContext({})

const handleFocus = e => {
  e.target.select()
  e.target.scrollIntoView({ inline: 'center', block: 'nearest' })
}

// localStorage.removeItem(`Telemachus-storedData`)

const getFromCache = async uri => {
  const { data } = await http.get(uri)
  return data

  // if (!isDev()) {
  //   const { data } = await http.get(uri)
  //   return data
  // }
  // try {
  //   const { userName } = JSON.parse(localStorage.getItem(`Telemachus-User`))
  //   if (!userName) {
  //     throw new Error('User not found')
  //   }
  //   let storedData = localStorage.getItem(`Telemachus-storedData`)
  //   storedData = storedData ? JSON.parse(storedData) : []
  //   let userData = storedData.find(d => d.userName === userName)
  //   if (!userData) {
  //     userData = {
  //       userName,
  //       itemList: []
  //     }
  //     storedData.push(userData)
  //   }
  //   let item = userData.itemList.find(i => i.uri === uri)
  //   if (!item) {
  //     item = {
  //       uri,
  //       data: null
  //     }
  //     userData.itemList.push(item)
  //   }
  //   if (!item.data) {
  //     const { data } = await http.get(uri)
  //     item.data = data
  //     localStorage.setItem(`Telemachus-storedData`, JSON.stringify(storedData))
  //   }

  //   return item.data
  // } catch (error) {
  //   // eslint-disable-next-line no-return-await
  //   const { data } = await http.get(uri)
  //   return data
  // }
}

export const ReportsProvider = ({ children }) => {
  const [data, setData] = React.useState({
    fields: []
  })
  const [historyData, setHistoryData] = React.useState({
    items: [],
    total: 0
  })
  const [loading, setLoading] = useState({ main: false, history: false })
  const [transferForm, setTransferForm] = useState(null)

  // create mode only
  const [bunkeringData, setBunkeringData] = useState([])

  const { id, type } = useParams()

  const { user, readOnlyMode, hidden } = useAuth()

  const modeState =
    type === 'edit' ? ModeState.Update : !type ? ModeState.ListOnly : ModeState.Create

  const tableData = useMemo(() => createView(data, historyData), [data, historyData])

  useEffect(() => {
    setBunkeringData(tableData[0]?.bunkeringData ?? [])
  }, [tableData])

  const statusBusinessId = tableData[0]?.event?.statusBusinessId

  const isEditable =
    !readOnlyMode && ModeState.isEditable(modeState) && statusBusinessId !== Status.Approved

  const { ask, showLoading, showModal } = useModal()

  const fetchData = async () => {
    if (modeState === ModeState.ListOnly) {
      return data
    }
    if (modeState === ModeState.Create) {
      const payload = await getFromCache(`/reports/${id}/fields`)
      const { performance, reportFields, ...newData } = payload
      const fields = createReportModel(
        margePerformanceFields(payload.reportFields, payload, id),
        margePerformanceFields(
          payload.relatedReport?.reportFields,
          payload,
          payload.relatedReport?.event?.id
        )
      )
      return {
        ...newData,
        // performanceFields: payload.performance,
        eventTypeName: `${payload.event?.eventTypeName ?? 'New'}`,
        fields
      }
    }
    if (modeState === ModeState.Update) {
      const payload = await getFromCache(`/reports/${id}`)
      const fields = createReportModel(
        margePerformanceFields(payload.reportFields, payload, payload.event.id),
        margePerformanceFields(
          payload.relatedReport?.reportFields,
          payload,
          payload.relatedReport?.event?.id
        )
      )
      const { performance, reportFields, ...newData } = payload
      return {
        ...newData,
        // performanceFields: performance,
        fields
      }
    }
    return data
  }

  const getData = async () => {
    setLoading(prev => ({ ...prev, main: true }))
    const payload = await fetchData()
    const storedData = reportState.getDraft(payload.event?.id)
    if (modeState === ModeState.Create && storedData) {
      for (const group of payload.fields) {
        if (!group.id) {
          group.fields.forEach(field => {
            const storedField = storedData.fields
              .find(g => g.id === group.id)
              ?.fields.find(f => f.id === field.id)
            if (storedField) {
              const index = group.fields.findIndex(f => f.id === field.id)
              if (index !== -1 && !field.readOnly) {
                group.fields[index] = {
                  ...field,
                  ...storedField
                }
              }
            }
          })
          continue
        }
        group.tanks.forEach(tank => {
          tank.fields.forEach(field => {
            const storedField = storedData.fields
              .find(g => g.id === group.id)
              ?.tanks.find(t => t.id === tank.id)
              ?.fields.find(f => f.id === field.id)
            if (storedField) {
              const index = tank.fields.findIndex(f => f.id === field.id)
              if (index !== -1) {
                tank.fields[index] = {
                  ...field,
                  ...storedField
                }
              }
            }
          })
        })
      }
    } else if (storedData) {
      reportState.deleteDraft(payload.event?.id)
    }
    if (payload.fields?.[0]?.fields.length) {
      if (payload.relatedReport?.reportFields)
        processConsumptions(payload.fields, payload.relatedReport.reportFields)

      processPerformanceData(payload.fields[0]?.fields)
    }
    setData(payload)
    if (payload.reportingProps) {
      const { reportingProps } = payload
      if (reportingProps?.internalTransferTargetTankId) {
        setTransferForm({
          value: reportingProps.internalTransferAmount,
          sourceTankId: reportingProps.internalTransferSourceTankId,
          targetTankId: reportingProps.internalTransferTargetTankId
        })
      }
    }
    setLoading(prev => ({ ...prev, main: false }))
  }

  const postReport = async (eventId, dto) => {
    const { data: reportId } = await http.post(`/reports/${eventId}`, dto)
    if (!reportId) {
      throw new Error()
    }
    return reportId
  }

  const onSave = () => {
    try {
      reportState.saveDraft(data?.event?.id, data)
      ask('Report saved as a draft.')
    } catch (e) {
      ask('Error saving report as a draft.')
    }
  }

  const updateReport = async () => {
    const dto = createDto(data.fields)
    if (data.event.eventTypeBusinessId === EventTypes.CommenceInternalTransfer) {
      dto.InternalTransferSourceTankId = transferForm?.sourceTankId
      dto.InternalTransferTargetTankId = transferForm?.targetTankId
      dto.InternalTransferAmount = parseFloat(transferForm?.value) || null
    }
    // eslint-disable-next-line no-unreachable
    setLoading(prev => ({ ...prev, main: true }))
    try {
      await http.patch(`/reports/${id}`, dto)
      reportState.deleteDraft(data?.event?.id)
      setLoading(prev => ({ ...prev, main: false }))
      await ask('Report saved successfully!')
      window.location.replace(window.location.href)
    } catch {
      setLoading(prev => ({ ...prev, main: false }))
      await ask('Failed to save the current report!')
    }
  }
  const getDate = async () => {
    const { data: date, reason: cancelReason } = await ask({
      component: <DateSelector />,
      title: 'Creation Date'
    })
    if (cancelReason === 'user' || !date) return
    return date
  }

  const createBunkeringPlanProjectedData = async date => {
    // eslint-disable-next-line prefer-const
    let { projectedBunkeringData, projectedBunkeringEventId } = await fetchProjectedBunkeringData(
      id
    )
    projectedBunkeringData = fillProjectedBunkeringData(
      data,
      createReportModel(
        projectedBunkeringData.reportFields,
        projectedBunkeringData.relatedReportFields?.reportFields
      ),
      projectedBunkeringData.event.bunkeringData
    )
    const dto = createDto(projectedBunkeringData)
    const reportId = await postReport(projectedBunkeringEventId, dto)
    return reportId
  }

  const createBunkeringCompleteDataReport = async date => {
    // eslint-disable-next-line prefer-const
    let { completeBunkeringData, completeBunkeringEventId } = await fetchCompleteBunkeringData(
      id,
      data
    )
    const dto = createDto(completeBunkeringData)
    const reportId = await postReport(completeBunkeringEventId, dto)
    return reportId
  }

  const createReport = async () => {
    const date = data.event.timestamp?.slice(0, 19) ?? (await getDate())
    const dto = createDto(data.fields)
    if (data.event.eventTypeBusinessId === EventTypes.CommenceInternalTransfer) {
      dto.InternalTransferSourceTankId = transferForm?.sourceTankId
      dto.InternalTransferTargetTankId = transferForm?.targetTankId
      dto.InternalTransferAmount = parseFloat(transferForm?.value) || null
    }
    setLoading(prev => ({ ...prev, main: true }))
    try {
      let reportId = await postReport(id, dto)
      reportState.deleteDraft(data?.event?.id)
      if (data.event.eventTypeBusinessId === EventTypes.BunkeringPlan) {
        reportId = await createBunkeringPlanProjectedData(date)
      }
      if (data.event.eventTypeBusinessId === EventTypes.CommenceBunkering) {
        reportId = await createBunkeringCompleteDataReport(date)
      }
      setLoading(prev => ({ ...prev, main: false }))
      await ask('Report created successfully!')
      return reportId
    } catch {
      setLoading(prev => ({ ...prev, main: false }))
      await ask('Failed to create report!')
      return null
    }
  }

  const handleSeed = () => {
    seedData(data)
    setData({ ...data })
  }

  const onSubmit = async () => {
    let isDirty = false
    let fuelType = null
    const settlingServiceTanks = data.fields.flatMap(g =>
      $.checkIf(g).isActualGroup() ? g.tanks?.filter(t => t.isSettOrServ) ?? [] : []
    )
    if (
      settlingServiceTanks.length &&
      settlingServiceTanks.every(t => t.fields.every(f => isNullOrEmpty(f)))
    ) {
      await ask('Not all settling service must be empty!')
      return
    }
    const eventTypeBusinessId = data?.event?.eventTypeBusinessId
    const bunkeringWeights = []
    if (data.event?.bunkeringData) {
      fuelType = data.event.bunkeringData.fuelType
    }

    // eslint-disable-next-line no-unreachable-loop
    for (const group of data.fields) {
      if ($.checkIf(group).isBunkeringGroup()) {
        bunkeringWeights.push(0)
      }
      if (!group.tanks) continue
      for (const tank of group.tanks) {
        if (tank.isSettOrServ) {
          if (tank.fields.every(f => isNullOrEmpty(f))) {
            continue
          }
        }
        if ($.checkIf(group).isBunkeringGroup()) {
          const fields = tank.fields.filter(f => !$.checkIf(f).isExcludedFromValidation())
          // eslint-disable-next-line eqeqeq

          if (fields.every(field => isNullOrEmpty(field) || parseFloat(field.value) === 0)) {
            continue
          }
        }
        for (const field of tank.fields) {
          const message = `Field \`${field.name ?? '?'}\` of tank \`${
            tank?.name ?? '?'
          }\` of group \`${groupRename(group.name) ?? '?'}\` should not be empty!`
          if ($.checkIf(group).isBunkeringGroup()) {
            if ($.checkIf(field).isWeightField()) {
              bunkeringWeights[bunkeringWeights.length - 1] += parseFloat(field.value)
            }
          }
          const isExcludedFromValidation = $.checkIf(field).isExcludedFromValidation()
          if (isExcludedFromValidation) continue
          if (!isNullOrEmpty(field)) continue
          if ($.checkIf(field).isViscosityField()) continue
          if (['No1 MGO T. (S)', 'NO2 MGO T (S)'].some(t => tank?.name === t)) {
            continue
          }
          if (
            ((eventTypeBusinessId === EventTypes.BunkeringPlan &&
              $.checkIf(group).isBunkeringGroup()) ||
              eventTypeBusinessId === EventTypes.BunkeringPlanProjected) &&
            $.checkIf(field).isBdnField()
          )
            continue
          if (
            fuelType &&
            EventTypes.BunkeringGroup.includes(eventTypeBusinessId) &&
            tank.fields.every(field => isNullOrEmpty(field)) &&
            !getGroupsOfFuelType(fuelType).includes(group.id)
          )
            continue

          await ask(message)
          return
        }
      }
    }
    if (
      [EventTypes.BunkeringPlan, EventTypes.BunkeringPlanProjected].includes(eventTypeBusinessId)
    ) {
      // console.log(bunkeringWeights)
      if (bunkeringWeights.every(w => w === 0)) {
        await ask(`At least one bunkering tank should have a non-zero weight!`)
        return
      }
    }
    for (const field of data.fields[0]?.fields ?? []) {
      if (data.relatedReport?.reportFields)
        processConsumptions(data.fields, data.relatedReport.reportFields)

      processPerformanceData(data.fields[0]?.fields, data.fields[0]?.fields.indexOf(field))
      if (!isNullOrEmpty(field)) continue
      if (!field.placeholder) {
        const fieldProps = performanceFields.find(f => f.name === field.validationKey)
        const required = fieldProps?.required
        if (required) {
          await ask(`${fieldProps.label} should not be empty!`)
          return
        }
        continue
      }
      isDirty = true
      field.value = field.placeholder
    }
    if (isDirty) {
      setData({ ...data })
      await ask(
        'Some fields were automatically filled with the values of the previous report. Check the values of the fields and repeat the submit process!'
      )
      return
    }
    const fields = tableData[0].restCols.concat(
      tableData[0].cols.flatMap(col => col.group?.tanks.flatMap(tank => tank.fields) ?? [])
    )
    const validationErrorfields = fields
      .filter(
        field => (field.fieldProps?.required && isNullOrEmpty(field)) || field.fieldProps?.isInvalid
      )
      .map(f => f.label)
      .join(', ')
    const hasValidationErrors = fields.some(
      field => (field.fieldProps?.required && isNullOrEmpty(field)) || field.fieldProps?.isInvalid
    )

    if (hasValidationErrors) {
      await ask(
        `A validation error occurred! Please check the integrity of the input values. (${truncateString2(
          validationErrorfields,
          100
        )})`
      )
      return
    }
    if (eventTypeBusinessId === EventTypes.CompleteBunkering) {
      for (const group of data.fields) {
        if (!group.tanks) continue
        const tanksWhereBunkeringIsApplied = group.tanks.filter(
          t => t.storage && t.fields.find(f => $.checkIf(f).isBdnField())?.defaultValue === ''
        )
        const isEqual = [
          ReportFields.BDN,
          ReportFields.SULPHUR_CONTENT,
          ReportFields.DENSITY
        ].every(fn => {
          const valuesToCompare = tanksWhereBunkeringIsApplied.flatMap(t =>
            t.fields
              .filter(f => f.validationKey === fn)
              .map(f => f.value?.toString().toLowerCase() ?? '')
          )
          return (
            valuesToCompare.length === 0 || valuesToCompare.every(v => v === valuesToCompare[0])
          )
        })
        if (!isEqual) {
          await ask(
            `All fuel specifications for ${groupRename(
              group.name
            )} must be consistent across all tanks where bunkering is being applied.`
          )
          return
        }
      }
    }

    if (eventTypeBusinessId === EventTypes.CommenceInternalTransfer) {
      if (!transferForm?.value) {
        await ask('Please complete the internal transfer process!')
      }
    }

    try {
      reportState.saveDraft(data?.event?.id, data)
    } catch (e) {
      //
    }

    if (modeState === ModeState.Update) {
      await updateReport()
      return
    }
    const reportId = await createReport()

    return reportId
  }

  const handleSettOrServ = (groupId, targetTankId, sourceTankId, debug) => () => {
    const targetFields = data.fields
      .find(group => group.id === groupId)
      ?.tanks.find(tank => tank.id === targetTankId)?.fields
    const sourceFields = data.fields
      .find(group => group.id === groupId)
      ?.tanks.find(tank => tank.id === sourceTankId)?.fields
    const poolFields = data.fields
      .find(g => $.checkIf(g).isPoolGroup(g.name))
      ?.tanks.find(tank => tank.id === targetTankId)?.fields
    for (const field of targetFields) {
      if (!sourceTankId) {
        field.value = ''
        field.maxValue = ''
        field.placeholder = ''
        field.relatedValue = ''
        continue
      }
      const sourceField = sourceFields?.find(f => f.validationKey === field.validationKey)
      if ($.checkIf(sourceField).isManagedField() || debug) {
        field.value = sourceField?.value ?? ''
        field.maxValue = sourceField?.maxValue
        field.placeholder = ''
        field.relatedValue = sourceField?.relatedValue
      } else if (
        $.checkIf(sourceField).isAutoCompletionField() ||
        $.checkIf(sourceField).isWeightField()
      ) {
        field.value = ''
        field.maxValue = ''
        field.placeholder = ''
        field.relatedValue = ''
      } else {
        field.value = ''
        field.maxValue = sourceField?.maxValue
        field.placeholder = sourceField?.value ?? ''
        field.relatedValue = sourceField?.relatedValue
      }
      const poolField = poolFields?.find(f => f.validationKey === field.validationKey)
      if (poolField) {
        poolField.value = field.value
        poolField.maxValue = field.maxValue
        poolField.placeholder = field.placeholder
        poolField.relatedValue = field.relatedValue
      }
    }
    setData({ ...data })
  }
  const handleCopy = (groupId, tankId) => () => {
    mergeGroups(data.fields, groupId, tankId)
    if (data.relatedReport?.reportFields)
      processConsumptions(data.fields, data.relatedReport.reportFields)
    processPerformanceData(data.fields[0]?.fields)
    setData({ ...data })
  }
  const handleClearFields = (groupId, tankId) => () => {
    const targetFields =
      data.fields
        .find(group => group.id === groupId)
        ?.tanks.find(tank => tank.id === tankId)
        ?.fields.filter(f => f.validationKey !== 'summary') ?? []
    for (const field of targetFields) {
      field.value = ''
      processData(data.fields, {
        fieldId: field.id,
        groupId,
        tankId,
        value: field.value
      })
    }
    setData({ ...data })
  }

  const handleBunkeringDataChange = bdnId => value => {
    const bdn = bunkeringData.find(b => b.id === bdnId)
    if (!bdn) {
      return
    }
    bdn.mt = value
    setBunkeringData([...bunkeringData])
  }
  const handleChange =
    groupId =>
    (tankId, fieldId, forceTargetWeight = false) =>
    e => {
      let value = e?.target
        ? e.target.type !== 'checkbox'
          ? e.target.value
          : e.target.checked
          ? '1'
          : '0'
        : e
      if ((value?.toString() ?? '').match(/^\s*\.\d+/)) value = `0${value.toString().trim()}`
      if (!groupId) {
        const targetField = data.fields[0]?.fields.find(field => field.id === fieldId)
        if (targetField) {
          targetField.value = value
        }
        // Mirror
        if (targetField.validationKey.includes('actual_consumption')) {
          const keys = targetField.validationKey.split('_')
          const fieldToPaste = data.fields[0]?.fields.find(
            f => f.validationKey === `${keys[0]}_pool_consumption_${keys[3]}`
          )
          if (fieldToPaste) {
            fieldToPaste.value = targetField.value
          }
        }
        if (data.relatedReport?.reportFields)
          processConsumptions(data.fields, data.relatedReport.reportFields)

        processPerformanceData(data.fields[0]?.fields, data.fields[0]?.fields.indexOf(targetField))
        setData({ ...data })
        return
      }
      // Mirror
      if ([1, 3].includes(groupId)) {
        const sourceField = data.fields
          .find(g => g.tanks?.some(t => t.id === tankId))
          ?.tanks.find(t => t.id === tankId)
          ?.fields.find(f => f.id === fieldId)

        const targetField = data.fields
          .find(g => g.id === groupId + 1)
          ?.tanks.find(t => t.id === tankId)
          ?.fields.find(f => f.validationKey === sourceField?.validationKey)

        if (targetField) targetField.value = value
      }
      processData(data.fields, {
        fieldId,
        groupId,
        tankId,
        value,
        forceTargetWeight
      })
      if (data.relatedReport?.reportFields)
        processConsumptions(data.fields, data.relatedReport.reportFields)

      processPerformanceData(data.fields[0]?.fields)

      setData({ ...data })
    }

  const useRows = useCallback(
    (i, key, tables) => {
      const readOnly = !isEditable || i > 0
      const reports = tableData
        .filter((report, index) => index >= i)
        .map((report, i) => {
          const { cols: _cols, restCols, ...restProps } = report
          const fields = restCols
            .filter(field =>
              tables?.some(table => table.cols?.some(col => col.name === field.validationKey))
            )
            .map(field => {
              if (!readOnly && i === 0 && !field.fieldProps.disabled) {
                field.fieldProps.onChange = handleChange()(null, field.id) ?? (() => undefined)
                field.fieldProps.onFocus = handleFocus
              }
              if (readOnly || i > 0) {
                field.fieldProps.disabled = true
              }
              return field
            })
          return {
            ...restProps,
            fields
          }
        })
      return reports
    },
    [tableData, isEditable, handleChange]
  )
  const validationErrorNames = useMemo(() => {
    const fields = tableData[0]?.restCols ?? []
    const groupFields = tableData[0]?.cols ?? []
    const fieldNames = fields.map(field => {
      if (field.variant === 'success') {
        return
      }
      if (!field.fieldProps?.isInvalid && !field.variant) {
        return
      }
      for (const tab of performanceTabs) {
        for (const subValue of tab.subValues ?? []) {
          for (const table of subValue.tables ?? tab.tables ?? []) {
            for (const col of table.cols ?? []) {
              if (col.name === field.validationKey) {
                return subValue.name
              }
            }
          }
        }
      }
      return undefined
    })
    const groupIds = groupFields.map(({ group, groupId }) => {
      for (const tank of group.tanks) {
        for (const field of tank.fields) {
          if (field.fieldProps?.isInvalid) {
            return groupId
          }
        }
      }
      return undefined
    })

    return fieldNames.concat(groupIds).filter(name => !!name)
  }, [tableData])
  const handleClear = () => {
    reportState.deleteDraft(data?.event?.id)
    getData()
  }
  useEffect(() => {
    showLoading(Object.values(loading).some(Boolean))
  }, [loading])

  const getRelatedBunkeringReport = async fieldValueId => {
    try {
      const report = await getFromCache(`/reports/bunkering/${fieldValueId}`)
      if (!report) {
        return null
      }
      return report
    } catch {
      return null
    }
  }

  const history = useHistory()
  const location = useLocation()
  const searchParams = new URLSearchParams(location.search)

  const [filters, setFilters] = useState({
    page: parseInt(searchParams.get('page')) || 1,
    pageSize: parseInt(searchParams.get('pageSize')) || PAGE_SIZE,
    conditionId: searchParams.get('conditionId') || null,
    events:
      searchParams
        .get('events')
        ?.split(',')
        .map(id => parseInt(id))
        .filter(Boolean) ?? [],
    subKey: groupReset(searchParams.get('subKey')) ?? null,
    key: groupReset(searchParams.get('key')) ?? null
  })

  useEffect(() => {
    const params = new URLSearchParams(location.search)
    if (filters.page > 1) {
      params.set('page', filters.page)
    } else {
      params.delete('page')
    }
    if (filters.pageSize > 0 && filters.pageSize !== PAGE_SIZE) {
      params.set('pageSize', filters.pageSize)
    } else {
      params.delete('pageSize')
    }
    if (filters.conditionId) {
      params.set('conditionId', filters.conditionId)
    } else {
      params.delete('conditionId')
    }
    if (filters.key) {
      params.set('key', groupRename(filters.key))
    } else {
      params.delete('key')
    }
    if (filters.subKey) {
      params.set('subKey', groupRename(filters.subKey))
    } else {
      params.delete('subKey')
    }
    if (filters.events.length) {
      params.set('events', filters.events.join(','))
    } else {
      params.delete('events')
      params.delete('key')
      params.delete('subKey')
    }
    const props = {
      pathname: location.pathname,
      search: params.toString()
    }
    if (
      !arraysAreIdentical(
        Array.from(params)
          .map(([key, value]) => ({ key, value }))
          .filter(p => !['subKey', 'key', 'events'].includes(p.key)),
        Array.from(searchParams)
          .map(([key, value]) => ({ key, value }))
          .filter(p => !['subKey', 'key', 'events'].includes(p.key)),
        'value'
      )
    ) {
      history.push(props)
    } else if (['subKey', 'key', 'events'].some(p => params.get(p) !== searchParams.get(p))) {
      history.replace(props)
    }
    // console.log(filters)
  }, [filters])

  useEffect(() => {
    if (!user?.userName) {
      return
    }
    let title = `${user.userName.toUpperCase()}`

    if (!id) {
      title += ` | Page ${filters.page || 1}`
    }

    if (id) {
      const event = tableData[0]?.event
      if (event) {
        title += ` - ${event.eventTypeName} Report (${formatDate(event.timestamp)})`
      }
    } else {
      title += ` - Reports`
      if (tableData.length) {
        title += ` (${formatDate(
          tableData.at(tableData.length - 1)?.event.timestamp
        )} - ${formatDate(tableData.at(0)?.event.timestamp)})`
      }
    }

    const sectionName =
      groups(false, hidden).find(g => g.key === filters.key)?.label ??
      performanceTabs
        .map(({ subValues, key }) =>
          subValues.map(subValue => ({
            ...subValue,
            key
          }))
        )
        .flat()
        .find(({ name, key }) => filters.subKey === name || filters.key === key)?.label

    if (sectionName) {
      title += ` # ${groupRename(sectionName)}`
    }
    document.title = title
  }, [user?.userName, filters.page, filters.key, filters.subKey, tableData])

  const fetchHistory = async () => {
    const { conditionId, page, pageSize } = filters
    const urlParams = new URLSearchParams()
    if (id) {
      if (modeState === ModeState.Update) {
        urlParams.set('reportId', id)
      } else {
        urlParams.set('eventId', id)
      }
    }
    if (modeState === ModeState.ListOnly) {
      urlParams.set('page', page)
    }
    urlParams.set('pageSize', pageSize)
    if (conditionId) {
      urlParams.set('conditionId', conditionId)
    }
    const paramsString = urlParams.toString()
    const data = await getFromCache(`/reports/history?${paramsString}`)
    return [
      data.items.map(i => {
        const { performance, relatedReport, reportingProps, ...newData } = i
        return {
          ...newData,
          reportFields: createReportModel(
            margePerformanceFields(i.reportFields, i, i.event.id),
            margePerformanceFields(i.relatedReport?.reportFields, i, i.relatedReport?.event.id)
          )
        }
      }),
      data.totalCount
    ]
  }

  const handleFilterChange = filterProps => {
    setFilters(prevFilters => {
      let newFilters = prevFilters
      if ('conditionId' in filterProps && filterProps.conditionId !== prevFilters.conditionId) {
        newFilters = {
          ...prevFilters,
          ...filterProps,
          page: 1,
          events: [],
          key: null,
          subKey: null
        }
      }
      if ('page' in filterProps && filterProps.page !== prevFilters.page) {
        newFilters = {
          ...prevFilters,
          ...filterProps,
          events: [],
          key: null,
          subKey: null
        }
      }
      if ('pageSize' in filterProps && filterProps.pageSize !== prevFilters.pageSize) {
        newFilters = {
          ...prevFilters,
          ...filterProps,
          events: [],
          key: null,
          subKey: null
        }
      }
      if ('key' in filterProps && filterProps.key !== prevFilters.key) {
        newFilters = {
          ...prevFilters,
          ...filterProps
        }
      }
      if ('subKey' in filterProps && filterProps.subKey !== prevFilters.subKey) {
        newFilters = {
          ...prevFilters,
          ...filterProps
        }
      }
      if (
        'events' in filterProps &&
        !arraysAreIdentical(filterProps.events, prevFilters.events, false)
      ) {
        newFilters = {
          ...prevFilters,
          ...filterProps
        }
      }
      if (!newFilters.events?.length) {
        newFilters.key = null
        newFilters.subKey = null
      }
      return newFilters
    })
  }

  useEffect(() => {
    if (!id) {
      return
    }
    getData()
    // handleFilterChange({ conditionId: null, page: 1 })
  }, [id])

  const handlePageClick = async data => {
    const currentPage = data.selected + 1
    handleFilterChange({ page: currentPage })
  }

  const handleClearFilters = () => handleFilterChange({ conditionId: null })

  const { page, pageSize, conditionId } = filters

  useEffect(() => {
    setLoading(prev => ({ ...prev, history: true }))
    fetchHistory()
      .then(([items, total]) => {
        for (const report of items) {
          if (report.reportFields?.[0].fields.length) {
            processPerformanceData(report.reportFields[0]?.fields)
          }
        }
        setHistoryData({
          items,
          total
        })
      })
      .catch(() => {
        return undefined
      })
      .finally(() => {
        setLoading(prev => ({ ...prev, history: false }))
      })
  }, [page, pageSize, conditionId])

  const conditions = useMemo(
    () =>
      [
        {
          label: 'At sea',
          value: 'd450dfe8-a736-4dd2-bcfa-14538522350d'
        },
        {
          label: 'Maneuvering',
          value: 'c80bc8cc-85ea-4a8c-a837-6f5221f2d1d2'
        },
        {
          label: 'At anchor',
          value: 'bb1686b1-84f3-41aa-b60d-58db4d88c199'
        },
        {
          label: 'Drifting',
          value: 'c6b2cead-d73b-4e49-89f5-53b1b4458516'
        },
        {
          label: 'Berthed',
          value: '7d960d64-67f2-4e93-bcb1-5b41f7c2c4b8'
        }
      ].map(c => ({
        ...c,
        active: filters.conditionId === c.value,
        onClick: () =>
          handleFilterChange({ conditionId: filters.conditionId === c.value ? null : c.value })
      })),
    [filters]
  )
  const selections = useMemo(() => {
    const { events, key, subKey } = filters
    return {
      events,
      key,
      subKey,
      isActive: (tabName, defaultClass = '') => {
        const tab = performanceTabs.find(tab =>
          tab.subValues?.some(subValue => subValue.name === tabName)
        )
        if (tab?.subValues?.some(subValue => subValue.tables?.length)) {
          const isActive = filters.subKey === tabName
          return isActive ? `selected ${defaultClass}` : defaultClass
        }
        const key = tab?.key ?? tabName
        const isActive = filters.key === key
        return isActive ? `selected ${defaultClass}` : defaultClass
      },
      onClick: (eventId, key, subKey) => e => {
        let filterProps = {}
        e?.stopPropagation()
        if (
          filters.key === key &&
          filters.subKey === subKey &&
          filters.events.some(id => id === eventId)
        ) {
          if (eventId === tableData[0]?.event.id && modeState !== ModeState.ListOnly) {
            filterProps = {
              key: null,
              subKey: null,
              events: []
            }
          } else {
            filterProps = {
              key,
              subKey,
              events: filters.events.filter(id => id !== eventId)
            }
          }
        } else if (filters.events.some(id => id === eventId)) {
          filterProps = {
            key,
            subKey
          }
        } else {
          filterProps = {
            events: [...filters.events, eventId],
            key,
            subKey
          }
        }
        handleFilterChange(filterProps)
      }
    }
  }, [filters.events, filters.key, filters.subKey, modeState])

  useEffect(() => {
    if (!data?.fields.length) return
    setFilters(prevFilters => {
      const events = prevFilters.events.filter(id => id !== data.event.id)
      events.push(data.event.id)
      return {
        ...prevFilters,
        events,
        key: prevFilters.key ?? groups(false, hidden).at(0).key,
        subKey: prevFilters.subKey ?? null
      }
    })
  }, [data])

  const onCopy = async () => {
    const fields = data.fields.flatMap(g =>
      (g.tanks?.flatMap(t => t.fields) ?? g.fields).map(f => ({
        fieldId: f.id,
        value: f.value
      }))
    )
    try {
      await navigator.clipboard.writeText(JSON.stringify(fields))
      await ask('JSON copied to clipboard.')
    } catch {
      await ask('Failed to copy JSON to clipboard.')
    }
  }

  const onPaste = async () => {
    const { action: cancelled, data: jsonString } = await showModal({
      title: 'Paste',
      defaultAction: ModalActions.CANCEL,
      component: <PastModalContainer />
    })
    if (cancelled) return
    try {
      const fields = JSON.parse(jsonString)
      for (const group of data.fields) {
        if (group.fields) {
          for (const field of group.fields) {
            field.value = fields.find(f => f.fieldId === field.id)?.value
          }
          continue
        }
        for (const tank of group.tanks) {
          for (const field of tank.fields) {
            field.value = fields.find(f => f.fieldId === field.id)?.value
          }
        }
      }
      setData({ ...data })
    } catch {
      await ask('Failed to read JSON.')
    }
  }

  const getTankId = async (tanks, title, required = false, highlightId = null) => {
    const { action: cancelled, data: tankId } = await showModal({
      title: `Telemachus`,
      defaultAction: ModalActions.CANCEL,
      component: (
        <TankSelector tanks={tanks} required={required} title={title} highlightId={highlightId} />
      )
    })
    if (cancelled) return null
    return tankId?.at(0) ?? null
  }
  const getTank = async (tanks, cb, title, required = false, highlightId = null) => {
    const targetTanks = tanks.filter(cb)
    if (targetTanks.length > 1) {
      const tatgetTankId = await getTankId(targetTanks, title, required, highlightId)
      if (!tatgetTankId) {
        return tatgetTankId
      }
      return tanks.find(t => t.id === tatgetTankId)
    }
    return targetTanks.at(0)
  }
  const handleAutoComplete = async validationKey => {
    const filteredData = bunkeringData.filter(filterFuelType(validationKey))
    if (!filteredData.length) return

    // utils
    const getField = (tank, key) => tank?.fields.find(f => f.validationKey === key)
    const getFieldValue = (tank, key) => getField(tank, key)?.value
    const getFieldNumericValue = (tank, key) => parseFloat(getField(tank, key)?.value) || 0

    const actualGroupId = filteredData.at(0).fuelType === 1 ? 1 : 3
    const actualGroup = data.fields.find(g => g.id === actualGroupId)
    const poolGroupId = filteredData.at(0).fuelType === 1 ? 2 : 4
    const poolGroup = data.fields.find(g => g.id === poolGroupId)
    const poolTanks = poolGroup?.tanks ?? []
    const actualTanks = actualGroup?.tanks ?? []

    const form = []
    for (const cons of filteredData) {
      if (!cons.mt) continue

      const getTitleElem = message => (
        <div className="py-2">
          <h6>{message}</h6>
          <p>
            <i>
              <small>{`Total cons: ${cons.mt.replace('.', ',')} MT, for BDN:${cons.bdn}`}</small>
            </i>
          </p>
        </div>
      )

      // select tanks

      const targetServTankHighlightedId = actualTanks
        .filter(t => t.serving)
        .find(t => {
          const volumeField = t.fields.find(f => f.validationKey === 'volume')
          const bdnField = t.fields.find(f => f.validationKey === 'bdn')
          if (volumeField.value !== volumeField.relatedValue && bdnField.value === cons.bdn) {
            return true
          }
          if (bdnField.value === cons.bdn) {
            return true
          }
          if (volumeField.value !== volumeField.relatedValue) {
            return true
          }
          return false
        })?.id

      const targetServTank = await getTank(
        poolTanks,
        t => t.serving,
        getTitleElem('Select a serving tank'),
        true,
        targetServTankHighlightedId
      )

      if (!targetServTank) {
        return
      }

      const targetSettTankHighlightedId = actualTanks
        .filter(t => t.settling)
        .find(t => {
          const volumeField = t.fields.find(f => f.validationKey === 'volume')
          const bdnField = t.fields.find(f => f.validationKey === 'bdn')
          if (volumeField.value !== volumeField.relatedValue && bdnField.value === cons.bdn) {
            return true
          }
          if (bdnField.value === cons.bdn) {
            return true
          }
          if (volumeField.value !== volumeField.relatedValue) {
            return true
          }
          return false
        })?.id

      const targetSettTank = await getTank(
        poolTanks,
        t => t.settling,
        getTitleElem(`Select a settling tank`),
        false,
        targetSettTankHighlightedId
      )

      if (targetSettTank === null) {
        return
      }

      // const targetStorageTanks = actualTanks.filter(t => {
      //   const volumeField = t.fields.find(f => f.validationKey === 'volume')
      //   const bdnField = t.fields.find(f => f.validationKey === 'bdn')
      //   if (volumeField.value !== volumeField.relatedValue && bdnField.value === cons.bdn) {
      //     return true
      //   }
      //   return false
      // })

      const targetStorageTankHighlightedId = actualTanks
        .filter(t => t.storage)
        .find(t => {
          const volumeField = t.fields.find(f => f.validationKey === 'volume')
          const bdnField = t.fields.find(f => f.validationKey === 'bdn')
          if (volumeField.value !== volumeField.relatedValue && bdnField.value === cons.bdn) {
            return true
          }
          if (volumeField.value !== volumeField.relatedValue) {
            return true
          }
          if (bdnField.value === cons.bdn) {
            return true
          }
          return false
        })?.id

      const targetStorTank = await getTank(
        poolTanks.filter(
          pt => actualTanks.some(tst => tst?.id === pt?.id) && getFieldValue(pt, 'bdn') === cons.bdn
        ),
        t => t.storage,
        getTitleElem(`Select a storage tank`),
        false,
        targetStorageTankHighlightedId
      )

      if (targetStorTank === null) {
        return
      }

      const targetTanks = [targetServTank, targetSettTank, targetStorTank]

      const internalTransferFormRequests = []

      for (const targetTank of targetTanks) {
        if (!targetTank) {
          continue
        }
        const targetWeight = getFieldNumericValue(targetTank, 'weight')
        const targetBdn = getFieldValue(targetTank, 'bdn')
        if (targetWeight <= 0 || targetBdn === cons.bdn) {
          continue
        }

        // process (if commingling)

        const availablePoolTanks = poolTanks.filter(
          poolTank =>
            !targetTanks.some(tt => tt.id === poolTank.id) &&
            getFieldValue(poolTank, 'bdn') === targetBdn
        )

        if (availablePoolTanks.length === 0) {
          throw new Error(`${targetTank.name} is not empty (${targetWeight} MT) and contains a different BDN number
              (${targetBdn}). No available tanks to perform an internal transfer so the existing
              product can be utilized.`)
        }

        const message = (
          <div className="py-2">
            <h6>
              {targetTank.name} is not empty ({targetWeight} MT) and contains a different BDN number
              ({targetBdn}). Please select a tank to perform an internal transfer so the existing
              product can be utilized.
            </h6>
          </div>
        )
        const targetPoolMixTank = await getTank(availablePoolTanks, () => true, message, true)

        if (!targetPoolMixTank) {
          return
        }

        const targetActualMixTank = actualTanks.find(
          actualTank => actualTank.id === targetPoolMixTank.id
        )

        const poolTankWeight = getFieldNumericValue(targetPoolMixTank, 'weight')
        const totalWeight = targetWeight + poolTankWeight

        const correctionFactor = getFuelProps(
          {
            fuelWeight: totalWeight,
            fuelDensity: getFieldNumericValue(targetActualMixTank, 'density'),
            tankTemperature: getFieldNumericValue(targetActualMixTank, 'tankTemperature')
          },
          ReportFields.WEIGHT
        )

        if (correctionFactor.totalObservedVolume > Number(targetPoolMixTank.capacity)) {
          throw new Error(
            `Target tank (${targetPoolMixTank.name}) overflow due to insufficient capacity. Try again with different options.`
          )
        }

        // Update pool tank weight
        const poolTankWeightField = getField(targetPoolMixTank, 'weight')

        poolTankWeightField.value = totalWeight

        // Zero out target tank weight
        const targetTankWeightField = getField(targetTank, 'weight')
        targetTankWeightField.value = 0

        const existingWeightRequest = internalTransferFormRequests.find(
          r =>
            r.groupId === poolGroupId &&
            r.tankId === targetPoolMixTank.id &&
            r.fieldId === poolTankWeightField.id
        )
        if (existingWeightRequest) {
          existingWeightRequest.value = totalWeight
        } else {
          internalTransferFormRequests.push({
            groupId: poolGroupId,
            tankId: targetPoolMixTank.id,
            fieldId: poolTankWeightField.id,
            value: totalWeight
          })
        }

        const targetBdnField = getField(targetPoolMixTank, 'bdn')

        targetBdnField.value = `${targetBdn}${
          targetBdnField.value !== targetBdn ? `, ${targetBdnField.value}` : ''
        }`

        if (targetBdnField.value !== targetBdn) {
          const targetCommField = getField(targetPoolMixTank, 'commingling')

          if (targetCommField) {
            targetCommField.value = '1'
            const existingCommRequest = internalTransferFormRequests.find(
              r =>
                r.groupId === poolGroupId &&
                r.tankId === targetPoolMixTank.id &&
                r.fieldId === targetCommField.id
            )
            if (existingCommRequest) {
              existingCommRequest.value = '1'
            } else {
              internalTransferFormRequests.push({
                groupId: poolGroupId,
                tankId: targetPoolMixTank.id,
                fieldId: targetCommField.id,
                value: '1'
              })
            }
          }
        }

        const existingBdnRequest = internalTransferFormRequests.find(
          r =>
            r.groupId === poolGroupId &&
            r.tankId === targetPoolMixTank.id &&
            r.fieldId === targetBdnField.id
        )
        if (existingBdnRequest) {
          existingBdnRequest.value = targetBdnField.value
        } else {
          internalTransferFormRequests.push({
            groupId: poolGroupId,
            tankId: targetPoolMixTank.id,
            fieldId: targetBdnField.id,
            value: targetBdnField.value
          })
        }
      }

      form.push(...internalTransferFormRequests)

      let remainingWeight =
        targetTanks.reduce((sum, t) => sum + getFieldNumericValue(t, 'weight'), 0) - cons.mt

      for (const targetPoolTank of targetTanks) {
        if (!targetPoolTank) {
          continue
        }
        const maxCapacity = parseFloat(targetPoolTank.capacity ?? 0)

        const bdnField = getField(targetPoolTank, 'bdn')

        bdnField.value = cons.bdn

        form.push({
          groupId: poolGroupId,
          tankId: targetPoolTank.id,
          fieldId: bdnField.id,
          value: cons.bdn
        })

        const targetActualTank = actualTanks.find(t => t.id === targetPoolTank.id)

        const correctionFactor = getFuelProps(
          {
            totalObservedVolume: maxCapacity,
            fuelDensity: getFieldNumericValue(targetActualTank, 'density'),
            tankTemperature: getFieldNumericValue(targetActualTank, 'tankTemperature')
          },
          ReportFields.VOLUME
        )

        const weightField = getField(targetPoolTank, 'weight')

        weightField.value = Math.min(remainingWeight, correctionFactor.fuelWeight)

        form.push({
          groupId: poolGroupId,
          tankId: targetPoolTank.id,
          fieldId: weightField.id,
          value: weightField.value.toFixed(3)
        })

        remainingWeight -= weightField.value
      }
      if (remainingWeight > 0) {
        throw new Error(`Total weight exceeds capacity by ${remainingWeight.toFixed(3)}`)
      }
    }
    for (const field of form) {
      handleChange(field.groupId)(field.tankId, field.fieldId, true)(field.value)
    }
    selections.onClick(
      tableData[0].eventId,
      `rob${validationKey.includes('hfo') ? 'Hfo' : 'Mgo'}Pool`
    )()
  }
  const memoedValue = React.useMemo(
    () => ({
      onCopy,
      onPaste,
      onSubmit,
      totalCount: historyData.total,
      handlePageClick,
      tableData,
      loading: Object.values(loading).some(Boolean),
      handleSeed,
      handleCopy,
      handleChange,
      handleBunkeringDataChange,
      useRows,
      onSave,
      handleSettOrServ,
      validationErrorNames,
      handleClear,
      getRelatedBunkeringReport,
      transferForm,
      setTransferForm,
      conditions,
      handleClearFilters,
      handleClearFields,
      page,
      modeState,
      selections,
      isEditable,
      bunkeringData,
      handleAutoComplete
    }),
    [
      modeState,
      tableData,
      historyData,
      loading,
      validationErrorNames,
      transferForm,
      conditions,
      page,
      selections,
      isEditable,
      bunkeringData
    ]
  )

  return <ReportsContext.Provider value={memoedValue}>{children}</ReportsContext.Provider>
}

const useReports = () => React.useContext(ReportsContext)

export default useReports
