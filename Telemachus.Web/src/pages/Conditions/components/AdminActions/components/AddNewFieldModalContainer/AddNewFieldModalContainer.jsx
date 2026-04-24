import { useEffect, useState } from 'react'
import { Button, ButtonGroup, Col, Container, Dropdown, Form, Row } from 'react-bootstrap'
import { Controller, useForm } from 'react-hook-form'
import ActionsContainer from 'src/components/Modal/Containers/ActionsContainer'
import { ModalActions, ModalDialogButtons } from 'src/components/Modal/ModalProps'
import useModal from 'src/components/Modal/useModal'
import MultiSelect from 'src/components/MultiSelect/MultiSelect'
import useAuth from 'src/context/useAuth'
import http from 'src/services/http'
import errorMessages from 'src/utils/errorMessages'
import { toCamelCase } from 'src/utils/stringUtils'

const AddNewFieldModalContainer = ({ setModalTitle, onAction }) => {
  const { debug } = useAuth()
  const fetchFields = async () => {
    const { data } = await http.get('/Admin/reportField')
    return data
  }
  const { ask } = useModal()
  const [field, setField] = useState(null)
  const [loading, setLoading] = useState(false)
  const [fields, setFields] = useState([])
  const [reportTypes, setReportTypes] = useState([])
  const [groups, setGroups] = useState([])
  const handleChange = e => {
    setField(fields.find(field => field.name === e.target.value) ?? null)
  }
  const [defaultValues, setDefaultValues] = useState({
    name: null,
    validationKey: null,
    description: null,
    groups: [],
    reportTypes: []
  })
  useEffect(() => {
    setDefaultValues({
      name: null,
      validationKey: null,
      description: null,
      groups: [],
      reportTypes: [],
      ...field
    })
    setModalTitle(field?.name ?? 'Create field')
  }, [field])
  const {
    reset,
    handleSubmit,
    register,
    control,
    watch,
    setValue,
    getValues,
    formState: { errors: validationErrors, isDirty, isSubmitted, isSubmitSuccessful }
  } = useForm({ defaultValues })
  const reValidate = async () => {
    setLoading(true)
    try {
      const { reportTypes, groups, fields } = await fetchFields()
      fields.forEach(field => {
        if (!field.validationKey) {
          field.validationKey = toCamelCase(field.name)
        }
      })
      setReportTypes(reportTypes)
      setGroups(groups)
      setFields(fields)
      if (field) {
        setField(fields.find(({ name }) => name === getValues('name')) ?? null)
      }
    } catch {
      await ask('An unknown error occurred!')
    } finally {
      setLoading(false)
    }
  }
  useEffect(() => {
    reValidate()
  }, [])
  const watchName = watch('name')
  const handleNameBlur = () => {
    if (field) return
    const validationKey = getValues('validationKey')
    if (!validationKey) {
      const name = getValues('name')
      if (name) {
        setValue('validationKey', toCamelCase(name))
      }
    }
  }
  const handleCancel = async () => {
    if (isDirty) {
      const canceled = await ask(
        'Are you sure you want to cancel? Any unsaved changes will be lost.',
        ModalDialogButtons.OKCancel,
        ModalActions.CANCEL
      )
      if (canceled) return
    }
    onAction?.(ModalActions.CANCEL)
  }
  const handleReset = async () => {
    if (isDirty) {
      const canceled = await ask(
        'Are you sure you want to reset? Any unsaved changes will be lost.',
        ModalDialogButtons.OKCancel,
        ModalActions.CANCEL
      )
      if (canceled) return
    }
    setDefaultValues({ ...defaultValues })
  }
  useEffect(() => {
    reset(defaultValues)
  }, [defaultValues])
  const handleFocus = event => event.target.select()
  const isValid = name => isSubmitted && !isSubmitSuccessful && !validationErrors[name]
  const isInvalid = name => isSubmitted && !isSubmitSuccessful && !!validationErrors[name]
  const validationMessage = name => validationErrors[name]?.message
  const a11yProps = (name, options) => ({
    'aria-invalid': isInvalid(name),
    'aria-describedby': `${name}-feedback`,
    'aria-errormessage': `${validationErrors[name]?.message ?? ''}`,
    id: name,
    isValid: isValid(name),
    isInvalid: isInvalid(name),
    ...register(name, { ...options })
  })
  const onSubmit = async formData => {
    try {
      if (!field) {
        await http.post('/Admin/reportField', formData)
      } else {
        await http.patch(`/Admin/reportField/${field.name}`, formData)
      }
    } catch {
      await ask('An unknown error occurred!')
      return undefined
    }
    await reValidate()
    if (!field) {
      setDefaultValues({ ...formData, name: '', validationKey: '', description: '' })
    }
  }
  const hasUniqueName = value => {
    return !fields.find(
      ({ name }) =>
        field?.name !== name && name?.trim().toLowerCase() === value?.trim().toLowerCase()
    )
  }
  const hasUniqueValidationKey = value => {
    return !fields.find(
      ({ validationKey }) =>
        field?.validationKey !== validationKey &&
        validationKey?.trim().toLowerCase() === value?.trim().toLowerCase()
    )
  }
  const handleDelete = hiddenOnly => async () => {
    if (!field) return
    const confirmMessage = hiddenOnly
      ? 'Are you sure you want to hide this field from the report form?'
      : 'Warning! If you delete this field, all history data associated with it will be lost! Press OK to confirm.'
    const rejected = await ask(confirmMessage, ModalDialogButtons.OKCancel, ModalActions.CANCEL)
    if (rejected) return
    try {
      await http.delete(
        `/Admin/reportField/${field.name}?hiddenOnly=${hiddenOnly ? 'true' : 'false'}`
      )
    } catch {
      await ask('An unknown error occurred!')
    }
    await reValidate()
  }
  return (
    <>
      <Container className="p-4">
        <Form id="fieldForm" onSubmit={handleSubmit(onSubmit)}>
          <fieldset disabled={loading}>
            <Row>
              <Form.Group as={Col} xs={12} className="my-2">
                <Form.Label htmlFor="field">Field</Form.Label>
                <Form.Select id={field} value={field?.name} onChange={handleChange}>
                  <option value="">Create new...</option>
                  <option value="-1" disabled>
                    - Grouped
                  </option>
                  {fields
                    .filter(({ groups, hidden }) => groups.length && !hidden)
                    .map(({ name }) => (
                      <option key={name} value={name}>
                        {name}
                      </option>
                    ))}
                  <option value="-1" disabled>
                    - Ungrouped
                  </option>
                  {fields
                    .filter(({ groups, hidden }) => !groups.length && !hidden)
                    .map(({ name }) => (
                      <option key={name} value={name}>
                        {name}
                      </option>
                    ))}
                  <option value="-1" disabled>
                    - Hidden
                  </option>
                  {fields
                    .filter(({ hidden }) => hidden)
                    .map(({ name }) => (
                      <option key={name} value={name}>
                        {name}
                      </option>
                    ))}
                </Form.Select>
                <Form.Text className="text-muted">Fields grouped by name.</Form.Text>
              </Form.Group>
            </Row>
            <Row>
              <Form.Group as={Col} md={6} className="my-2">
                <Form.Label htmlFor="name">Name</Form.Label>
                <Form.Control
                  type="text"
                  className="form-control"
                  onFocus={handleFocus}
                  {...a11yProps('name', {
                    validate: value => hasUniqueName(value) || 'This name already exists!',
                    required: errorMessages.validation.required,
                    maxLength: { value: 128, message: errorMessages.validation.maxLength?.(128) }
                  })}
                  onBlur={handleNameBlur}
                />
                <Form.Control.Feedback type="invalid">
                  {validationMessage('name')}
                </Form.Control.Feedback>
              </Form.Group>
              <Form.Group as={Col} md={6} className="my-2">
                <Form.Label htmlFor="validationKey">Validation Key</Form.Label>
                <Form.Control
                  disabled={!watchName}
                  type="text"
                  className="form-control"
                  onFocus={handleFocus}
                  {...a11yProps('validationKey', {
                    validate: value =>
                      hasUniqueValidationKey(value) || 'This validation key already exists!',
                    required: errorMessages.validation.required
                  })}
                />
                <Form.Text className="text-muted">Required for development purposes.</Form.Text>
                <Form.Control.Feedback type="invalid">
                  {validationMessage('validationKey')}
                </Form.Control.Feedback>
              </Form.Group>
            </Row>
            <Row>
              <Form.Group as={Col} xs={12} className="my-2">
                <Form.Label htmlFor="description">Description</Form.Label>
                <Form.Control
                  type="text"
                  className="form-control"
                  onFocus={handleFocus}
                  {...a11yProps('description')}
                />
                <Form.Text className="text-muted">
                  It will be shown as a label column on the report form.
                </Form.Text>
                <Form.Control.Feedback type="invalid">
                  {validationMessage('description')}
                </Form.Control.Feedback>
              </Form.Group>
            </Row>
            <Row>
              <Form.Group as={Col} md={6} className="my-2">
                <Form.Label htmlFor="reportTypes">Report Types</Form.Label>
                <Controller
                  rules={{
                    required: errorMessages.validation.required
                  }}
                  control={control}
                  name="reportTypes"
                  defaultValue={[]}
                  render={({ field: { onChange, value, ...field } }) => (
                    <MultiSelect
                      {...field}
                      onSelectOption={selectedOption =>
                        onChange(selectedOption.map(option => option.id))
                      }
                      onRemoveValue={removedValue =>
                        onChange(value.filter(id => id !== removedValue.id))
                      }
                      onClear={() => onChange([])}
                      isDisabled={loading}
                      value={reportTypes.filter(option => value.includes(option.id))}
                      isValid={isValid('reportTypes')}
                      isInvalid={isInvalid('reportTypes')}
                      zIndex={10111}
                      aria-errormessage={validationErrors.reportTypes?.message}
                      id="reportTypes"
                      placeholder="Report Types"
                      getOptionLabel={option => option.name}
                      isOptionSelected={(option, selectValue) =>
                        selectValue.some(({ id }) => id === option.id)
                      }
                      options={reportTypes}
                    />
                  )}
                />
                {isInvalid('reportTypes') && (
                  <Form.Control.Feedback
                    style={{
                      display: 'block'
                    }}
                    type="invalid">
                    {validationMessage('reportTypes')}
                  </Form.Control.Feedback>
                )}
              </Form.Group>
              <Form.Group as={Col} md={6} className="my-2">
                <Form.Label htmlFor="reportTypes">Groups</Form.Label>
                <Controller
                  control={control}
                  name="groups"
                  defaultValue={[]}
                  render={({ field: { onChange, value, ...field } }) => (
                    <MultiSelect
                      {...field}
                      onSelectOption={selectedOption =>
                        onChange(selectedOption.map(option => option.id))
                      }
                      onRemoveValue={removedValue =>
                        onChange(value.filter(id => id !== removedValue.id))
                      }
                      onClear={() => onChange([])}
                      isDisabled={loading}
                      value={groups.filter(option => value.includes(option.id))}
                      isValid={isValid('groups')}
                      isInvalid={isInvalid('groups')}
                      zIndex={10111}
                      aria-errormessage={validationErrors.groups?.message}
                      id="groups"
                      placeholder="Groups"
                      getOptionLabel={option => option.fieldGroupName}
                      isOptionSelected={(option, selectValue) =>
                        selectValue.some(({ id }) => id === option.id)
                      }
                      options={groups}
                    />
                  )}
                />
                {isInvalid('groups') && (
                  <Form.Control.Feedback
                    style={{
                      display: 'block'
                    }}
                    type="invalid">
                    {validationMessage('groups')}
                  </Form.Control.Feedback>
                )}
              </Form.Group>
            </Row>
          </fieldset>
        </Form>
      </Container>
      <ActionsContainer dialogButtons={null}>
        <Button disabled={loading || !isDirty} type="submit" form="fieldForm" variant="primary">
          Submit
        </Button>
        <Button disabled={loading || !isDirty} variant="light" onClick={handleReset}>
          Reset
        </Button>
        {field && (
          <Dropdown as={ButtonGroup}>
            {field.hasValues ? (
              <Button
                disabled={loading || field.hidden}
                onClick={handleDelete(true)}
                variant="danger">
                Make Hidden
              </Button>
            ) : (
              <Button disabled={loading} onClick={handleDelete(false)} variant="danger">
                Delete
              </Button>
            )}
            <Dropdown.Toggle
              disabled={loading || !field.hasValues}
              split
              variant="danger"
              id="dropdown-split-basic"
            />
            <Dropdown.Menu>
              <Dropdown.Item disabled={loading} onClick={handleDelete(false)}>
                Delete
              </Dropdown.Item>
            </Dropdown.Menu>
          </Dropdown>
        )}
        <Button disabled={loading} variant="light" onClick={handleCancel}>
          Back
        </Button>
      </ActionsContainer>
    </>
  )
}

export default AddNewFieldModalContainer
