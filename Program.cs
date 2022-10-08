using System.CommandLine;
namespace SVGConvert;

public class SvgTool
{
    public static int Main(string[] args)
    {
        // string[] argsTest = {"-h","--input","/home/adalberto/Vídeos/linux.svg","-w","12"};
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("SVGConvert v1.0\t\'" + DateTime.Now + "\'\n");
        Console.ResetColor();
        string output = "";
        FileInfo fileInfo = null;
        Format fmt= Format.Png;
        int anchoS = 0, altoS=0, escaleS=0;
        Option<FileInfo> fileOption = new Option<FileInfo>(aliases: new []{"--input","-i"}, description:"Ruta de archivo de entrada");
        Option<int> anchoOption = new Option<int>(aliases:new []{"--width","-w"}, description:"ancho de la imagen");
        Option<int> altoOption = new Option<int>(aliases:new []{"--height","-ht"},description:"Alto de la imagen");
        Option<int> escaleOption = new Option<int>(aliases:new []{"--escale","-e"},description:"Cuantas veces se multiplicara las dimensiones");
        Option<Format> formatOption = new Option<Format>(aliases:new []{"--format","-f"},description:"Formato de salida");
        Option<string> salidaOption = new Option<string>(aliases:new []{"--output","-o"},description:"Ruta de salida");
        
        RootCommand rootCommand = new RootCommand();
        rootCommand.AddOption(fileOption);
        rootCommand.AddOption(anchoOption);
        rootCommand.AddOption(altoOption);
        rootCommand.AddOption(escaleOption);
        rootCommand.AddOption(formatOption);
        rootCommand.AddOption(salidaOption);
        rootCommand.Description = Help();
        rootCommand.SetHandler((input, ancho, alto, escale, formato,salida) =>
        {
            fileInfo = input;
            anchoS = ancho;
            altoS = alto;
            escaleS = escale;
            fmt = formato;
            output = salida;
        },fileOption,anchoOption,altoOption,escaleOption,formatOption,salidaOption);

        if (rootCommand.Invoke(args) == 1)
        {
            return 1;
        }
        if (fileInfo == null)
        {
            return 1;
        }

        Utilidad ut = new Utilidad();
        ut.SetDefaultExtension(true);
        ut.ProcessSvg(fileInfo,anchoS,altoS,output,fmt,escaleS);
        return 0;
    }
    
    /// <summary>
    /// retorna un string que tiene los ejemplos de uso de la herramienta
    /// </summary>
    /// <returns></returns>
    private static string Help()
    {
        string example = ""
                         + "SVGConvert es una Herramienta por linea de comandos\npara convertir un documento SVG en imagen.\n"
                         + "Formatos Soportados: BMP, EMF, EXIF, JPEG, PNG, TIFF, WMF.\n"
                         + "Ejemplos:\n"
                         + "1==>\tSVGConvert -i \"/home/user/Documents/svgDocument.svg\"\n"
                         + "2==>\tSVGConvert -i \"/home/user/Documents/svgDocument.svg\" --escale 10\n"                         
                         + "3==>\tSVGConvert -i \"/home/user/Documents/svgDocument.svg\" -w 1080\n"
                         + "4==>\tSVGConvert -i \"/home/user/Documents/svgDocument.svg\" -ht 500\n"
                         + "5==>\tSVGConvert --input \"/home/user/Documents/svgDocument.svg\" -o \"/home/user/Imagenes/svgImg.png\" -f png\n"
                         + "6==>\tSVGConvert -i \"/home/user/Documents/svgDocument.svg\" --output \"/home/user/Imagenes/svgImg.bmp\" -f bmp --escale 5\n"
                         + "7==>\tSVGConvert -i \"/home/user/Documents/svgDocument.svg\" -o \"/home/user/Imagenes/svgImg.jpeg\" --format jpeg --width 1000 -ht 1000\n"
                         + "8==>\tSVGConvert --input \"/home/user/Documents/svgDocument.svg\" --output \"/home/user/Imagenes/svgImg.jpeg\" --format tiff --escale 5 --width 1000 --height 1000";
        return example;
    }
}