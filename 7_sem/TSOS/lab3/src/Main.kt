package by.kukhatskavolets.lab3

import java.awt.image.BufferedImage
import java.io.File
import javax.imageio.ImageIO

const val PI: Float = 3.1415927f

const val channels: Int = 3

fun main() {
	mdIfNot("output")

	val imagePath = "src/shrek.png"

	val boxOutput = "output/blur_box.jpg"
	val gaussOutput = "output/blur_gauss.jpg"
	val medianOutput = "output/blur_median.jpg"
	val sobelOutput = "output/sobel.jpg"
	val graySobelOutput = "output/graySobel.jpg"
	val sharrsOutput = "output/sharrs.jpg"
	val graySharrsOutput = "output/graySharrs.jpg"
	val pruittsOutput = "output/pruitts.jpg"
	val grayPruittsOutput = "output/grayPruitts.jpg"

	val imageArray = loadImageAsArray(imagePath)
//
	val boxBlurredImage = applyBoxBlur(imageArray, 80)
	saveImageFromArray(boxBlurredImage, boxOutput)

	val gaussBlurredImage = applyGaussianBlur(imageArray, 10, 10.0)
	saveImageFromArray(gaussBlurredImage, gaussOutput)

	val medianBlurredImage = applyMedianBlur(imageArray, 10)
	saveImageFromArray(medianBlurredImage, medianOutput)

	val coloredGradients = compareAllGradientOperators(imageArray)
	val grayGradients = compareAllGradientOperatorsGray(imageArray)

	saveImageFromArray(coloredGradients["Sobel"]!!, sobelOutput)
	saveImageFromArray(coloredGradients["Sharr"]!!, sharrsOutput)
	saveImageFromArray(coloredGradients["Pruitt"]!!, pruittsOutput)

	saveImageFromArray(grayGradients["Sobel"]!!, graySobelOutput)
	saveImageFromArray(grayGradients["Sharr"]!!, graySharrsOutput)
	saveImageFromArray(grayGradients["Pruitt"]!!, grayPruittsOutput)

}

private fun loadImageAsArray(imagePath: String): Array<Array<IntArray>> {
	val img = ImageIO.read(File(imagePath))
	val width = img.width
	val height = img.height
	val imageArray = Array(height) { Array(width) { IntArray(3) } }

	for (y in 0..<height) {
		for (x in 0..<width) {
			val rgb = img.getRGB(x, y)
			imageArray[y][x][0] = (rgb shr 16) and 0xff
			imageArray[y][x][1] = (rgb shr 8) and 0xff
			imageArray[y][x][2] = rgb and 0xff
		}
	}

	return imageArray
}

private fun saveImageFromArray(imageArray: Array<Array<IntArray>>, outputPath: String) {
	val (height, width) = extractDimensions(imageArray)
	val img = BufferedImage(width, height, BufferedImage.TYPE_INT_RGB)

	for (y in 0..<height) {
		for (x in 0..<width) {
			val r = imageArray[y][x][0]
			val g = imageArray[y][x][1]
			val b = imageArray[y][x][2]
			img.setRGB(x, y, (r shl 16) or (g shl 8) or b)
		}
	}

	ImageIO.write(img, "jpg", File(outputPath))
}

private fun mdIfNot(path: String): File {
	val folder = File(path)
	if (!folder.exists()) {
		folder.mkdirs()
	}
	return folder
}

fun extractDimensions(imageArray: Array<Array<IntArray>>): Pair<Int, Int> {
	val height = imageArray.size
	val width = imageArray[0].size
	return height to width
}