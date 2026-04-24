/* eslint-disable no-promise-executor-return */
/* eslint-disable no-await-in-loop */
/* eslint-disable no-plusplus */
import { PDFDocument, rgb, StandardFonts } from 'pdf-lib'
import * as pdfjsLib from 'pdfjs-dist'

pdfjsLib.GlobalWorkerOptions.workerSrc =
  'https://unpkg.com/pdfjs-dist@latest/build/pdf.worker.min.mjs'

const processPdf = async (file, compressImage) => {
  if (!file) return file

  return new Promise((resolve, reject) => {
    const reader = new FileReader()
    reader.readAsArrayBuffer(file)
    reader.onload = async () => {
      try {
        const pdfData = new Uint8Array(reader.result)
        const pdf = await pdfjsLib.getDocument({ data: pdfData }).promise
        const totalPages = pdf.numPages

        const newPdf = await PDFDocument.create()
        let hasImages = false

        for (let i = 1; i <= totalPages; i++) {
          const page = await pdf.getPage(i)
          const textContent = await page.getTextContent()

          // Render page on a canvas
          const viewport = page.getViewport({ scale: 1 })
          const canvas = document.createElement('canvas')
          const context = canvas.getContext('2d')
          canvas.width = viewport.width
          canvas.height = viewport.height
          await page.render({ canvasContext: context, viewport }).promise

          const imageData = context.getImageData(0, 0, canvas.width, canvas.height)
          const hasImage = imageData.data.some((pixel, index) => index % 4 !== 3 && pixel !== 255)

          if (hasImage) {
            hasImages = true
            const blob = await new Promise(resolve => canvas.toBlob(resolve, 'image/jpeg'))
            const compressedImageBase64 = await compressImage(blob)

            const pdfPage = newPdf.addPage([canvas.width, canvas.height])
            const jpgImage = await newPdf.embedJpg(compressedImageBase64)
            pdfPage.drawImage(jpgImage, {
              x: 0,
              y: 0,
              width: canvas.width,
              height: canvas.height
            })

            // Add text back to the page
            const { width, height } = pdfPage.getSize()
            const textFont = await newPdf.embedFont(StandardFonts.Helvetica)
            textContent.items.forEach(text => {
              pdfPage.drawText(text.str, {
                x: text.transform[4],
                y: height - text.transform[5],
                size: 12,
                font: textFont,
                color: rgb(0, 0, 0)
              })
            })
          } else {
            const copiedPage = await newPdf.copyPages(pdf, [i - 1])
            newPdf.addPage(copiedPage[0])
          }
        }

        if (!hasImages) {
          resolve(new Blob([pdfData], { type: 'application/pdf' }))
          return
        }

        const compressedPdfBlob = await newPdf.save()
        resolve(new Blob([compressedPdfBlob], { type: 'application/pdf' }))
      } catch (error) {
        reject(error)
      }
    }
  })
}

export default processPdf
