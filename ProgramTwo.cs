using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;

namespace ImageProcessingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            string inputDirectory = "images";
            string outputDirectory = "processed_images";

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string[] imageFiles = Directory.GetFiles(inputDirectory, "*.jpg");

            // Crear un array de hilos
            Thread[] threads = new Thread[imageFiles.Length];

            // Iniciar cada hilo para procesar una imagen
            for (int i = 0; i < imageFiles.Length; i++)
            {
                string imagePath = imageFiles[i];
                threads[i] = new Thread(() => ProcessImage(imagePath, outputDirectory));
                threads[i].Start();
            }

            // Esperar a que todos los hilos terminen
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            stopwatch.Stop();
            Console.WriteLine($"Todas las imágenes se han procesado en {stopwatch.Elapsed.TotalSeconds} segundos.");
        }

        public static void ProcessImage(string imagePath, string outputDirectory)
        {
            try
            {
                using (var originalImage = new Bitmap(imagePath))
                {
                    var scaledImage = ScaleImage(originalImage, 0.5);
                    var grayscaleImage = ConvertToGrayscale(scaledImage);

                    string outputFilePath = Path.Combine(outputDirectory, Path.GetFileName(imagePath));
                    grayscaleImage.Save(outputFilePath);

                    Console.WriteLine($"Imagen procesada y guardada: {outputFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar la imagen {imagePath}: {ex.Message}");
            }
        }

        public static Bitmap ScaleImage(Image image, double scale)
        {
            int newWidth = (int)(image.Width * scale);
            int newHeight = (int)(image.Height * scale);
            var scaledImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(scaledImage))
            {
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return scaledImage;
        }

        public static Bitmap ConvertToGrayscale(Bitmap original)
        {
            var grayscaleImage = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color originalColor = original.GetPixel(x, y);
                    int grayScaleValue = (int)((originalColor.R * 0.3) + (originalColor.G * 0.59) + (originalColor.B * 0.11));
                    Color grayColor = Color.FromArgb(grayScaleValue, grayScaleValue, grayScaleValue);
                    grayscaleImage.SetPixel(x, y, grayColor);
                }
            }

            return grayscaleImage;
        }
    }
}
