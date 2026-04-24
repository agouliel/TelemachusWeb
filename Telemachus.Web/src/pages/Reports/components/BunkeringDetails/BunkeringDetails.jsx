/* eslint-disable react/no-unstable-nested-components */
import {
  faCircleNotch,
  faClose,
  faDownload,
  faTrash,
  faUpload
} from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { saveAs } from 'file-saver'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import {
  Accordion,
  Badge,
  Button,
  ButtonGroup,
  ListGroup,
  OverlayTrigger,
  Placeholder,
  Popover
} from 'react-bootstrap'
import { withRouter } from 'react-router-dom'
import sanitize from 'sanitize-filename'
import InputValueFormLayout from 'src/components/InputValueFormLayout/InputValueFormLayout'
import { ModalActions, ModalDialogButtons } from 'src/components/Modal/ModalProps'
import useModal from 'src/components/Modal/useModal'
import useAuth from 'src/context/useAuth'
import useReports from 'src/context/useReports'
import isNumeric from 'src/pages/Reports/utils/isNumeric'
import http from 'src/services/http'
import delay from 'src/utils/delay'
import formatDate from 'src/utils/formatDate'
import resizeImage from 'src/utils/resizeImage'

const BunkeringDetails = ({
  fieldValueId,
  files,
  history,
  icon,
  tankId,
  debug,
  onData,
  groupKey
}) => {
  const { getRelatedBunkeringReport } = useReports()
  const { ask, showLoading, showModal } = useModal()

  const [specs, setSpecs] = useState({
    relatedReport: {
      id: null,
      timestamp: null,
      bunkeringData: null
    },
    files
  })

  const handleReportClick = reportId => () => {
    history.push(`/reports/edit/${reportId}`)
  }
  const [loading, setLoading] = useState(false)
  const { debugMessage, readOnlyMode } = useAuth()

  const variant = useMemo(() => {
    if (!specs.files.length) return undefined
    if (loading) return undefined
    if (!specs.relatedReport.timestamp) return undefined
    if (!specs.relatedReport.bunkeringData) return undefined
    if (!specs.relatedReport.bunkeringData.supplier) return 'danger'
    if (specs.files.every(file => !file.id)) return 'danger'
    if (specs.files.some(file => !file.id)) return 'warning'
  }, [specs, loading])

  const defaultActiveKeys = useMemo(() => {
    const keys = []
    if (!variant) return keys
    if (specs.files.some(file => !file.id)) {
      keys.push('0')
    }
    if (!specs.relatedReport.bunkeringData.supplier) {
      keys.push('1')
    }
    return keys
  }, [variant])

  const [mounted, setMounted] = useState(true)

  const fetch = async () => {
    await delay()
    const report = await getRelatedBunkeringReport(fieldValueId)
    return report
  }
  const refresh = () => {
    setLoading(true)
    return fetch()
      .then(report => {
        if (!mounted) return
        if (!report) return
        const data = {
          relatedReport: {
            id: report.id,
            timestamp: report.event.timestamp,
            bunkeringData: report.event.bunkeringData
          },
          files: specs.files.map(file => {
            const attachment = report.event.attachments.find(
              a => a.documentType?.code === file.documentCode
            )
            return { ...file, fileName: attachment?.fileName, id: attachment?.id }
          })
        }
        setSpecs(data)
        onData?.(data)
      })
      .finally(() => {
        if (!mounted) return
        setLoading(false)
      })
  }
  const handleSupplier = async () => {
    if (!specs.relatedReport.bunkeringData?.id) return
    const { id } = specs.relatedReport.bunkeringData
    const { action: cancelled, data } = await showModal({
      title: 'Bunkering Details',
      defaultAction: null,
      dialogProps: {
        maxWidth: 'lg'
      },
      component: (
        <InputValueFormLayout
          label="Supplier"
          rules={{
            required: 'This field is required.'
          }}
          defaultValue={specs.relatedReport.bunkeringData.supplier ?? ''}
        />
      )
    })
    if (cancelled) return
    showLoading(true)
    try {
      await http.patch(`/reports/bunkering/${id}/supplier`, {
        supplier: data.value
      })
      await refresh()
      showLoading(false)
    } catch {
      showLoading(false)
      ask('Failed to update data!')
    }
  }
  const handleNamedAmount = async () => {
    if (!specs.relatedReport.bunkeringData?.id) return
    const { id } = specs.relatedReport.bunkeringData
    const { action: cancelled, data } = await showModal({
      title: 'Bunkering Details',
      defaultAction: null,
      dialogProps: {
        maxWidth: 'lg'
      },
      component: (
        <InputValueFormLayout
          label="Named Amount (mt)"
          defaultValue={specs.relatedReport.bunkeringData.namedAmount ?? ''}
          type="number"
          rules={{
            validate: form => {
              return isNumeric(form) || !form || 'Invalid value.'
            }
          }}
        />
      )
    })
    if (cancelled) return
    showLoading(true)
    try {
      await http.patch(`/reports/bunkering/${id}/namedAmount`, {
        namedAmount: data.value
      })
      await refresh()
      showLoading(false)
    } catch {
      showLoading(false)
      ask('Failed to update data!')
    }
  }
  const upload = async (documentCode, file) => {
    showLoading(true)
    const isPDF = file.type === 'application/pdf'
    if (!isPDF) {
      try {
        const compressedFile = await resizeImage(file)
        if (compressedFile.size < file.size) {
          file = compressedFile
        }
      } catch (error) {
        console.error(error)
      }
    }
    const maxSizeInBytes = 20 * 1024 * 1024
    if (file.size > maxSizeInBytes) {
      showLoading(false)
      ask('File size exceeds the limit of 20MB!')
      return
    }
    const formData = new FormData()
    formData.append('files', file)
    try {
      await http.post(
        `/reports/attachment/${specs.relatedReport.bunkeringData.id}/${documentCode}`,
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        }
      )
      await refresh()
      await ask('File uploaded successfully!')
    } catch (error) {
      showLoading(false)
      if (error.request?.status === 413) {
        ask('File size exceeds the limit of 1.2MB!')
        return
      }
      ask('Failed to upload file!')
    }
  }
  const awaitFileState = useRef(null)
  const fileInputRef = useRef(null)
  const onFileChange = event => {
    const file = event.target.files[0]
    if (awaitFileState.current) awaitFileState.current.resolve(file)
    awaitFileState.current = null
  }
  const handleUpload = documentCode => async () => {
    if (fileInputRef.current) {
      fileInputRef.current.value = ''
      fileInputRef.current.click()
    }
    const file = await new Promise((resolve, reject) => {
      awaitFileState.current = { resolve, reject }
    })
    await upload(documentCode, file)
  }
  const handleDelete = attachmentId => async () => {
    const rejected = await ask(
      'Are you sure you want to delete this file?',
      ModalDialogButtons.OKCancel,
      ModalActions.CANCEL
    )
    if (rejected) return
    showLoading(true)
    try {
      await http.delete(`/reports/attachment/${attachmentId}`)
      await refresh()
      await ask('File deleted successfully!')
    } catch {
      showLoading(false)
      await ask('Failed to delete file!')
    }
  }
  useEffect(() => {
    return () => setMounted(false)
  }, [])
  useEffect(() => {
    refresh()
  }, [groupKey])
  const [show, setShow] = useState(false)
  const handleToggle = useCallback(
    isOpen => {
      setShow(isOpen)
      if (!isOpen) return
      if (loading) return
      refresh()
    },
    [loading]
  )
  const handlePreview = attachmentId => async () => {
    showLoading(true)
    try {
      const {
        data: { token }
      } = await http.get(`/download`)
      showLoading(false)
      window.open(`/api/download/${attachmentId}?token=${token}`, '_blank')
    } catch {
      showLoading(false)
      ask('Error downloading file')
    }
  }
  const handleDownload = async () => {
    try {
      showLoading(true)
      const { headers, data } = await http.get(
        `/reports/attachment/bunkering/${specs.relatedReport.bunkeringData.id}`,
        {
          responseType: 'arraybuffer'
        }
      )
      showLoading(false)
      const contentType = headers['content-type']
      const p = /.*filename=(.+);.*/i
      const match = headers['content-disposition']?.match(p)
      const name = match ? match[1] : 'download'
      const file = new File([data], name, {
        type: contentType || 'application/force-download'
      })
      saveAs(file, sanitize(name))
    } catch {
      showLoading(false)
      ask('Error downloading file')
    }
  }
  return (
    <OverlayTrigger
      onToggle={handleToggle}
      show={show}
      trigger={['click']}
      placement="auto"
      overlay={
        <Popover className="d-print-none" style={{ zIndex: 12113 }}>
          <Popover.Body className="p-0">
            <div className="d-flex justify-content-between align-items-center px-3 py-2">
              <h6 className="dropdown-header me-auto">Bunkering Details</h6>
              <div>
                <Button
                  className="pe-0"
                  disabled={loading || !specs.files.some(f => !!f.id)}
                  title="Download all"
                  variant="link"
                  onClick={handleDownload}>
                  <FontAwesomeIcon icon={faDownload} />
                </Button>
                <Button
                  className="pe-0 me-0"
                  title="Hide"
                  variant="link"
                  onClick={() => handleToggle(false)}>
                  <FontAwesomeIcon icon={faClose} />
                </Button>
              </div>
            </div>
            {!loading ? (
              <ListGroup className="py-0 list-group-flush" as="ul">
                <Accordion flush defaultActiveKey={defaultActiveKeys}>
                  <Accordion.Item eventKey="0">
                    <Accordion.Header>Documents</Accordion.Header>
                    <Accordion.Body>
                      {specs.files.map(file => (
                        <div
                          key={file.documentCode}
                          className="d-flex justify-content-between align-items-center">
                          <div
                            className="me-auto"
                            style={{
                              overflow: 'hidden',
                              textOverflow: 'ellipsis'
                            }}>
                            <div className="fw-bold">{file.label}</div>
                            {!file.id ? <i>Required file is missing!</i> : file.fileName}
                          </div>
                          <ButtonGroup size="sm" className="ms-3">
                            {file.id ? (
                              <Button title="Download" variant="link">
                                <FontAwesomeIcon
                                  icon={faDownload}
                                  onClick={handlePreview(file.id)}
                                />
                              </Button>
                            ) : (
                              <Button
                                disabled={readOnlyMode}
                                title="Upload"
                                variant="link"
                                onClick={handleUpload(file.documentCode)}>
                                <FontAwesomeIcon icon={faUpload} />
                              </Button>
                            )}
                            <Button
                              title="Delete"
                              disabled={!file.id || readOnlyMode}
                              variant="link"
                              onClick={handleDelete(file.id)}>
                              <FontAwesomeIcon icon={faTrash} />
                            </Button>
                          </ButtonGroup>
                          <div className="hr" />
                          <input
                            ref={fileInputRef}
                            onChange={onFileChange}
                            hidden
                            accept="*/*"
                            type="file"
                          />
                        </div>
                      ))}
                    </Accordion.Body>
                  </Accordion.Item>
                  <Accordion.Item eventKey="1">
                    <Accordion.Header>Specifications</Accordion.Header>
                    <Accordion.Body>
                      {specs.relatedReport.bunkeringData && (
                        <dl className="row">
                          <dt className="col-sm-6">BDN</dt>
                          <dd className="col-sm-6">{specs.relatedReport.bunkeringData.bdn}</dd>
                          <dt className="col-sm-6">
                            Supplier
                            {!specs.relatedReport.bunkeringData.supplier && (
                              <Badge className="ms-2" bg="danger">
                                !
                              </Badge>
                            )}
                          </dt>
                          <dd className="col-sm-6">
                            {specs.relatedReport.bunkeringData.supplier ?? (
                              <i className="text-muted">TBA</i>
                            )}
                            <div>
                              <Button
                                disabled={!specs.relatedReport.bunkeringData?.id || readOnlyMode}
                                onClick={handleSupplier}
                                className="px-0 mx-0"
                                size="sm"
                                variant="link">
                                Update
                              </Button>
                            </div>
                          </dd>
                          <dt className="col-sm-6">Port</dt>
                          <dd className="col-sm-6">{specs.relatedReport.bunkeringData.portName}</dd>
                          <dt className="col-sm-6 text-truncate">Date</dt>
                          <dd className="col-sm-6">
                            {formatDate(specs.relatedReport.bunkeringData.timestamp)}
                          </dd>
                          <dt className="col-sm-6 text-truncate">Weight (Named/mt)</dt>
                          <dd className="col-sm-6">
                            {specs.relatedReport.bunkeringData.namedAmount || '-'}{' '}
                            <div>
                              <Button
                                disabled={!specs.relatedReport.bunkeringData?.id || readOnlyMode}
                                onClick={handleNamedAmount}
                                className="px-0 mx-0"
                                size="sm"
                                variant="link">
                                Update
                              </Button>
                            </div>
                          </dd>
                          <dt className="col-sm-6 text-truncate">Weight (ROB/mt)</dt>
                          <dd className="col-sm-6">
                            {specs.relatedReport.bunkeringData.robAmount || '-'}
                          </dd>
                          <dt className="col-sm-6 text-truncate">Weight (Consumption)</dt>
                          <dd className="col-sm-6">
                            {specs.relatedReport.bunkeringData.robAmountDiff || '-'}
                          </dd>
                          <dt className="col-sm-6 text-truncate">Density</dt>
                          <dd className="col-sm-6">
                            {specs.relatedReport.bunkeringData.density || '-'}
                          </dd>
                          <dt className="col-sm-6 text-truncate">Sulphur Content</dt>
                          <dd className="col-sm-6">
                            {specs.relatedReport.bunkeringData.sulphurContent || '-'}
                          </dd>
                          <dt className="col-sm-6 text-truncate">Commingled</dt>
                          <dd className="col-sm-6">
                            {(() => {
                              const data = specs.relatedReport.bunkeringData.tanks.find(
                                t => t.tankId === tankId
                              )?.comminglingData
                              if (data) {
                                return (
                                  <Button
                                    disabled
                                    className="p-0"
                                    variant="link"
                                    onClick={handleReportClick(data.bunkeringCompleteReportId)}>
                                    {data.bdn}
                                  </Button>
                                )
                              }
                              return <i>None</i>
                            })()}
                          </dd>
                        </dl>
                      )}
                    </Accordion.Body>
                  </Accordion.Item>
                </Accordion>
                <Button
                  className="d-none"
                  disabled
                  variant="link"
                  size="sm"
                  onClick={handleReportClick(
                    specs.relatedReport.bunkeringData?.bunkeringCompleteReportId
                  )}>
                  More Details
                </Button>
              </ListGroup>
            ) : (
              <div className="p-3">
                <div>
                  <Placeholder as="div" animation="glow" aria-hidden="true">
                    <Placeholder xs={7} /> <Placeholder xs={2} /> <Placeholder xs={2} />
                  </Placeholder>
                  <Placeholder as="div" animation="glow" aria-hidden="true">
                    <Placeholder xs={7} />
                  </Placeholder>
                </div>
                <div>
                  <Placeholder as="div" animation="glow" aria-hidden="true">
                    <Placeholder xs={7} /> <Placeholder xs={2} /> <Placeholder xs={2} />
                  </Placeholder>
                  <Placeholder as="div" animation="glow" aria-hidden="true">
                    <Placeholder xs={7} />
                  </Placeholder>
                </div>
              </div>
            )}
          </Popover.Body>
        </Popover>
      }>
      <Button
        onDoubleClick={debug ? () => console.log(specs) : undefined}
        title={debugMessage(
          `bunkeringDataId: ${specs.relatedReport.bunkeringData?.id ?? '?'}`,
          !specs.relatedReport.timestamp ? 'Bunkering plan is missing!' : undefined
        )}
        disabled={!specs.relatedReport.timestamp}
        className="position-relative text-primary"
        size="sm"
        variant="link">
        <FontAwesomeIcon icon={loading ? faCircleNotch : icon} spin={loading} />
        {!loading && !!variant && (
          <span
            style={{ zIndex: 11111 }}
            className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
            !<span className="visually-hidden">missing data</span>
          </span>
        )}
      </Button>
    </OverlayTrigger>
  )
}

export default withRouter(BunkeringDetails)
