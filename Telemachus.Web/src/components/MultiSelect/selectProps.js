export default function selectProps({ isValid, isInvalid }) {
  return {
    styles: {
      control: (base, { isFocused }) => ({
        ...base,
        ...(isFocused && {
          boxShadow: '0 0 0 0.2rem rgba(0, 123, 255, 0.25)'
        }),
        '&.is-invalid': {
          borderColor: 'var(--danger)',
          paddingRight: 'calc(0.75em + 2.3125rem) !important',
          background:
            "url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' width='4' height='5' viewBox='0 0 4 5'%3e%3cpath fill='%23343a40' d='M2 0L0 2h4zm0 5L0 3h4z'/%3e%3c/svg%3e\") right 0.75rem center/8px 10px no-repeat, #fff url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' fill='none' stroke='%23dc3545' viewBox='0 0 12 12'%3e%3ccircle cx='6' cy='6' r='4.5'/%3e%3cpath stroke-linejoin='round' d='M5.8 3.6h.4L6 6.5z'/%3e%3ccircle cx='6' cy='8.2' r='.6' fill='%23dc3545' stroke='none'/%3e%3c/svg%3e\") center right 1.75rem/calc(0.75em + 0.375rem) calc(0.75em + 0.375rem) no-repeat",
          ...(isFocused && {
            boxShadow: '0 0 0 0.2rem rgba(220, 53, 69, 0.25)'
          })
        },
        '&.is-invalid:hover': {
          borderColor: 'var(--danger)'
        },
        '&.is-valid': {
          borderColor: 'var(--success)',
          paddingRight: 'calc(1.5em + 0.75rem) !important',
          backgroundImage:
            "url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' width='8' height='8' viewBox='0 0 8 8'%3e%3cpath fill='%2328a745' d='M2.3 6.73L.6 4.53c-.4-1.04.46-1.4 1.1-.8l1.1 1.4 3.4-3.8c.6-.63 1.6-.27 1.2.7l-4 4.6c-.43.5-.8.4-1.1.1z'/%3e%3c/svg%3e\")",
          backgroundRepeat: 'no-repeat',
          backgroundPosition: 'right calc(0.375em + 0.1875rem) center',
          backgroundSize: 'calc(0.75em + 0.375rem) calc(0.75em + 0.375rem)',
          ...(isFocused && {
            boxShadow: '0 0 0 0.2rem rgba(40, 167, 69, 0.25)'
          })
        },
        '&.is-valid:hover': {
          borderColor: 'var(--success)'
        }
      })
    },
    classNames: {
      control: () => `${isInvalid ? ' is-invalid' : ''}${isValid ? ' is-valid' : ''}`
    }
  }
}
