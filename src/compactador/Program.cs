using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

class ImageCompressor
{
    public static void CompressImage(string inputPath, string outputPath, long quality)
    {
        using (FileStream fs = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
        using (Image image = Image.FromStream(fs, useEmbeddedColorManagement: false, validateImageData: false))
        using (Bitmap bitmap = new Bitmap(image))
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters encoderParams = new EncoderParameters(1);
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = qualityParam;

            bitmap.Save(outputPath, jpgEncoder, encoderParams);
        }
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }
        return null;
    }

    static void Main()
    {
        string diretorioDeImagens = "D:\\imagens-compactar\\";
        string diretorioDeSaida = "D:\\imagens-compactar\\Saida\\";

        long quality = 50L; // Qualidade entre 0 e 100 (menor valor = maior compressão)

        IniciarCompactacao(diretorioDeImagens, diretorioDeSaida, quality);

        Console.WriteLine("Imagem compactada com sucesso!");
    }

    private static void IniciarCompactacao(string diretorioDeImagens, string diretorioDeSaida, long quality)
    {
        var diretorio = new DirectoryInfo(diretorioDeImagens);

        if (!Directory.Exists(diretorioDeSaida))
        {
            Directory.CreateDirectory(diretorioDeSaida);
        }

        foreach (var arquivo in diretorio.GetFiles())
        {
            string saida = Path.Combine(diretorioDeSaida, arquivo.Name);
            try
            {
                CompressImage(arquivo.FullName, saida, quality);
                Console.WriteLine($"Imagem compactada: {arquivo.FullName} -> {saida}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar {arquivo.Name}: {ex.Message}");
            }
        }

        foreach (var subDiretorio in diretorio.GetDirectories())
        {
            if (diretorio.FullName.ToUpper().Contains("SAIDA"))
            {
                return;
            }

            string novoDiretorioDeSaida = Path.Combine(diretorioDeSaida, subDiretorio.Name);
            IniciarCompactacao(subDiretorio.FullName, novoDiretorioDeSaida, quality);
        }
    }
}