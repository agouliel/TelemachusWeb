/* eslint-disable jsx-a11y/control-has-associated-label */
import { memo } from 'react'
import { Button } from 'react-bootstrap'
import { useController } from 'react-hook-form'
import { getValidationRules } from 'src/pages/Conditions/components/AdminActions/components/ManageEventModalContainer/useEventTypes'
import useTanks from 'src/pages/Conditions/components/AdminActions/components/ManageTankModalContainer/useTanks'

const ActionButton = memo(
  ({ index, name, control, disabled, tankId, action, caption, variant }) => {
    const { onAction } = useTanks()
    const {
      field: { value: formValue, ref }
    } = useController({
      name: `forms.${index}.${name}`,
      control,
      rules: getValidationRules(name, index)
    })

    if (typeof variant === 'function') {
      variant = variant(formValue)
    }
    if (typeof disabled === 'function') {
      disabled = disabled(formValue)
    }
    if (typeof action === 'function') {
      action = action(formValue)
    }
    if (typeof caption === 'function') {
      caption = caption(formValue)
    }

    return (
      <Button
        variant={variant}
        ref={ref}
        disabled={disabled}
        id={`action-${index}-${action}`}
        onClick={onAction?.(action, tankId)}>
        {caption}
      </Button>
    )
  },
  (prevProps, nextProps) => {
    return prevProps.tankId === nextProps.tankId
  }
)

ActionButton.displayName = 'ActionButton'

export default ActionButton
