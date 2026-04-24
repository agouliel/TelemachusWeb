function getRandomNumber(min, max) {
  return Math.floor(Math.random() * (max - min + 1)) + min
}

const delay = async (min = 200, max = 700) => {
  return new Promise(resolve => {
    setTimeout(() => {
      resolve()
    }, getRandomNumber(min, max))
  })
}

export default delay
