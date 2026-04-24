// eslint-disable-next-line react/function-component-definition
import { faFileWord, faRotateLeft } from '@fortawesome/free-solid-svg-icons'
import { format } from 'date-fns'
import ButtonGroup from 'react-bootstrap/ButtonGroup'
import Dropdown from 'react-bootstrap/Dropdown'

import { ModalActions, ModalDialogButtons } from 'src/components/Modal/ModalProps'

import StatementFactContainerWithProvider from 'src/components/StatementFact/StatementFactContainer/StatementFactContainer'

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { useEffect } from 'react'
import { Button } from 'react-bootstrap'
import useModal from 'src/components/Modal/useModal'
import useAuth from 'src/context/useAuth'
import useStatements from 'src/context/useStatements'
import http from 'src/services/http'

const formattedRange = statement => {
  let label = ''
  const d1 = new Date(statement.fromDate)
  const d2 = new Date(statement.toDate)
  if (!Number.isNaN(d1)) {
    label += format(d1, 'dd/MM/yy')
    if (!Number.isNaN(d2)) {
      label += ' → '
    }
  }
  if (!Number.isNaN(d2)) {
    label += format(d2, 'dd/MM/yy')
  }
  if (!label.length) {
    label = 'Document'
  }
  if (!statement.completed) {
    label += ' (Last modified)'
  }
  return label
}
// eslint-disable-next-line react/function-component-definition
export default function StatementList({ disabled }) {
  const { user } = useAuth()
  const { showModal, ask, showLoading } = useModal()
  const { statements, fetchStatements, loading } = useStatements()
  useEffect(() => {
    if (!user) return
    fetchStatements()
  }, [])
  const onDownload = statementId => async e => {
    if (e) {
      e.preventDefault()
    }
    await showModal({
      lg: true,
      title: 'Document Export',
      defaultAction: null,
      component: <StatementFactContainerWithProvider id={statementId} />
    })
    await fetchStatements()
  }
  const restore = statementId => async e => {
    e.stopPropagation()
    e.preventDefault()
    const rejected = await ask(
      'Archived documents are read only, if you want to restore the last completed statement (make it editable again) and delete current (draft) please confirm your action!',
      ModalDialogButtons.OKCancel,
      ModalActions.CANCEL
    )
    if (rejected) return
    await http.patch(`/Statement/${statementId}/0`)
    await onDownload(statementId)()
  }

  const createNewDocument = async () => {
    const lastStatementId = statements.find(statement => !statement.completed)?.id
    if (!lastStatementId) {
      await onDownload()()
      return
    }
    const rejected = await ask(
      'Do you want to archive the last statement and create a blank one? (Yes)\n\nArchived documents are read only and they cannot be edited, press No if you want to continue editing the last one!',
      ModalDialogButtons.YesNo,
      ModalActions.YES
    )
    if (!rejected) {
      await http.patch(`/Statement/${lastStatementId}`)
    }
    await onDownload()()
  }

  return (
    <Dropdown as={ButtonGroup}>
      <button
        type="button"
        className="btn btn-primary text-nowrap nav-button me-0"
        disabled={disabled || !statements.some(statement => !statement.completed)}
        style={{ whiteSpace: 'nowrap' }}
        onClick={onDownload()}>
        <FontAwesomeIcon className="me-2" icon={faFileWord} />
        {statements.some(statement => !statement.completed) ? 'SOF' : 'SOF'}
      </button>
      <Dropdown.Toggle
        as={Button}
        disabled={disabled || !statements.length}
        split
        id="dropdown-split-basic"
        className="ms-0 position-relative btn btn-primary text-nowrap nav-button"
      />
      <Dropdown.Menu
        style={{ zIndex: 11115 }}
        popperConfig={{
          strategy: 'fixed',
          onFirstUpdate: () => window.dispatchEvent(new CustomEvent('scroll'))
        }}>
        <Dropdown.Item disabled={disabled} onClick={createNewDocument}>
          Create new...
        </Dropdown.Item>
        {statements.length && (
          <>
            <Dropdown.Divider />
            <Dropdown.Header>Recent Documents</Dropdown.Header>
          </>
        )}
        {statements.map((statement, index) => (
          <Dropdown.Item
            className="d-flex justify-content-between align-items-start"
            onClick={onDownload(statement.id)}
            key={statement.id}
            href="#">
            <div className="ms-2 me-auto">
              <div className="fw-bold">{statement.charterParty || 'Untitled'}</div>
              {formattedRange(statement)}
            </div>
            {index < 2 && statement.completed && (
              <Button
                size="sm"
                title="Restore"
                className="ms-auto"
                style={{ whiteSpace: 'nowrap' }}
                variant="light"
                onClick={restore(statement.id)}>
                <FontAwesomeIcon icon={faRotateLeft} />
              </Button>
            )}
          </Dropdown.Item>
        ))}
      </Dropdown.Menu>
    </Dropdown>
  )
}
