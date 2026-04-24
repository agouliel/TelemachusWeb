import Resizer from 'react-image-file-resizer'

const resizeImage = file =>
  new Promise((resolve, reject) => {
    Resizer.imageFileResizer(
      file,
      1024, // target width
      768, // target height
      'webp',
      80, // quality
      0, // rotation
      uri => {
        resolve(uri)
      },
      'file', // 'base64' for debugging
      1024, // max width
      768 // max height
    )
  })

export default resizeImage
