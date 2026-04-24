const getOr = keyName => {
  switch (keyName) {
    case 'ArrowRight':
    case 'ArrowLeft':
      return 'x'
    default:
      return 'y'
  }
}

const getNextInput = (currentInput, keyName) => {
  const position = getOr(keyName)
  const allInputs = document.querySelectorAll('input:not(:disabled):not(.rbt-input-hint)')
  let inputsArray = Array.from(allInputs)
  const sourceRect = currentInput?.getBoundingClientRect()[position === 'y' ? 'left' : 'top']
  if (position === 'y') {
    inputsArray = inputsArray.filter(input => {
      const targetRect = input.getBoundingClientRect().left
      return sourceRect === targetRect
    })
  } else {
    inputsArray = inputsArray.filter(input => {
      const targetRect = input.getBoundingClientRect().top
      return sourceRect === targetRect
    })
  }
  inputsArray.sort((input1, input2) => {
    const left1 = input1.getBoundingClientRect()[position === 'x' ? 'left' : 'top']
    const left2 = input2.getBoundingClientRect()[position === 'x' ? 'left' : 'top']
    if (left1 < left2) {
      return -1
    }
    if (left1 > left2) {
      return 1
    }
    return 0
  })
  const currentIndex = inputsArray.indexOf(currentInput)
  const targetIndex =
    keyName === 'ArrowRight' || keyName === 'ArrowDown' ? currentIndex + 1 : currentIndex - 1
  if (targetIndex < 0 || targetIndex >= inputsArray.length) return
  const nextInput = inputsArray[targetIndex]
  return nextInput
}

const handleKeyPress = currentInput => event => {
  currentInput =
    typeof currentInput === 'object' ? currentInput : document.getElementById(currentInput)
  switch (event.key) {
    case 'ArrowRight':
    case 'ArrowLeft':
    case 'ArrowUp':
    case 'ArrowDown':
    case 'Enter':
      event.preventDefault()
      break
    default:
      return
  }
  const nextInput = getNextInput(currentInput, event.key === 'Enter' ? 'ArrowDown' : event.key)
  if (nextInput) {
    nextInput.focus()
  }
}

export default handleKeyPress
