using System.Drawing;
using System.Drawing.Imaging;
using Svg;

namespace SVGConvert;

/// <summary>
/// 
/// </summary>
public class Utilidad
{
    public Utilidad()
    {
    }
    private FileInfo _fileInfo;
    private String _output;
    private Format _formatoSalida = Format.Bmp;
    private float _ancho=0;
    private float _alto=0;
    private bool _extensionAddDefault = false;
    private float _escale = 1;
    
    /// <summary>
    /// Convertir un SVG en imagen.
    /// </summary>
    /// <param name="fileInfo">objeto que contiene la ruta del archivo svg</param>
    /// <param name="ancho">Ancho que tendra la imagen de salida</param>
    /// <param name="alto">Alto que tendra la imagen de salida</param>
    /// <param name="salida">Ruta de salida, donde se guardara la imagen</param>
    /// <param name="formatoSalida">El formato de la imagen</param>
    /// <param name="escale">Es cuantas veces se multiplicara las dimensiones de la imagen. 1 es por defecto</param>
    public void ProcessSvg(FileInfo fileInfo, float ancho, float alto, string salida,Format formatoSalida,float escale = 1)
    {
        //Validaciones para evitar problemas
        if (fileInfo.Exists==false)
        {
            Log($"El archivo \"{fileInfo.FullName}\" no existe.");
            return;
        }
        if (fileInfo.Extension!=".svg")
        {
            Log($"El tipo de archivo es invalido. \"{fileInfo.Extension}\" is invalid.");
            return;
        }

        if (ancho <= 0 || alto <= 0)
        {
            if (ancho > 0)
            {
                alto = ancho;
            }else if (alto > 0)
            {
                ancho = alto;
            }
            else
            {
                Log($"Las dimensiones son erroneas X:{ancho}, Y:{alto}. Tiene que ser mayor a 0");
                return;
            }
        }

        string fmt = "" + formatoSalida;
        //Si la salida del archivo es nulo o vacia entonces ocupar la ruta de la imagen principal
        if (String.IsNullOrEmpty(salida))
        {
            Log($"El archivo de salida \"{salida}\" es NULO o Vacio.\nSe dejar por defecto la siguiente ruta:");
            int index = fileInfo.FullName.LastIndexOf(fileInfo.Extension, StringComparison.Ordinal);
            salida = $"{fileInfo.FullName.Remove(index)}.{fmt.ToLower()}";
            Log(salida);
        }else if (String.IsNullOrWhiteSpace(salida))
        {
            Log($"El archivo de salida \"{salida}\" es NULO o Espacio en blanco.\nSe dejar por defecto la siguiente ruta:");
            int index = fileInfo.FullName.LastIndexOf(fileInfo.Extension, StringComparison.Ordinal);
            salida = $"{fileInfo.FullName.Remove(index)}.{fmt.ToLower()}";
            Log(salida);
        }
        else
        {
            if (_extensionAddDefault)
                salida = salida + "."+ fmt.ToLower();
        }

        if (escale < 1)
        {
            Log($"Escala invalida: {escale}, \"Tiene que ser mayor a cero\". Se asignara el valor de 1");
            this._escale = 1;
        }
        else
        {
            this._escale = escale;
        }
        
        //Asignando Valores
        _ancho = ancho;
        _alto = alto;
        this._formatoSalida = formatoSalida;
        this._fileInfo = fileInfo;
        this._output = salida;
        //Convirtiendo SVG 
        if (SvgConvert())
        {
            Log("Proceso Exitoso!");
        }
        else
        {
            Log("Error!!!");
        }
    }
    private bool SvgConvert()
    {
        if (_fileInfo==null)
        {
            Log($"File is Null{_fileInfo}");
            return false;
        }
        try
        {
            SvgDocument svgDocument = SvgDocument.Open(_fileInfo.FullName);
            //Si se modifican las dimensiones del svg
            if (_ancho > 0 || _alto > 0)
            {
                svgDocument.Width = _ancho*_escale;
                svgDocument.Height = _alto*_escale;
            }else if (_escale > 1)
            {
                svgDocument.Width = svgDocument.ViewBox.Width*_escale;
                svgDocument.Height = svgDocument.ViewBox.Height*_escale;
            }
            if (svgDocument == null)
            {
                Log($"El objeto es null: {_fileInfo.FullName}");
                return false;
            }
            bool exist = new FileInfo(_output).Exists;
            
            using Bitmap img = svgDocument.Draw();
            try
            {
                img.Save(_output, Formato(_formatoSalida));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(exist ? "Se modifico el archivo: {0}" : "Se creo el archivo: {0}", _output);
                Console.ResetColor();
                return true;
            }
            catch (System.Runtime.InteropServices.ExternalException exp)
            {
                Log("Error al Guardar: " + exp.Message+" || "+" ExternalException. "+"CodeError: "+exp.ErrorCode);
                return false;
            }catch(NullReferenceException exp)
            {
                Log("Error al Guardar: " + exp.Message+" || "+ "NullReferenceException");
                return false;
            }
        }
        catch (System.Xml.XmlException ex)
        {
            Log("Archivo Invalido: "+ ex.Message);
            return false;
        }
    }

    private ImageFormat Formato(Format fmt)
    {
        return fmt switch
        {
            Format.Bmp => ImageFormat.Bmp,
            Format.Jpeg => ImageFormat.Jpeg,
            Format.Png => ImageFormat.Png,
            Format.Tiff => ImageFormat.Tiff,
            Format.Emf => ImageFormat.Emf,
            Format.Exif => ImageFormat.Exif,
            Format.Wmf => ImageFormat.Wmf,
            _ => ImageFormat.Png
        };
    }

    /// <summary>
    /// Si SVGTool Agregara automaticamente la extension si el usuario no la especifica
    /// </summary>
    /// <param name="isDefaultExtension">true?: false?</param>
    public void SetDefaultExtension(bool isDefaultExtension)
    {
        this._extensionAddDefault = isDefaultExtension;
    }
    private static void Log(object message)
    {
        Console.WriteLine(message);
    }
}