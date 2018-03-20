using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebGrupo3S.ValidationHelper
{
    public class Logs
    {
        #region Private Attributes
        private string folder = "";
        private string path = "";
        protected string BRINCO = "\n";
        protected List<string> lErrores = new List<string>();

        #endregion
        #region Propierties
        public string FullPath
        {
            get { return path + "/" + folder; }
        }
        #endregion

        public Logs(string folder_, string path)
        {
            folder = folder_;
            this.path = path;
        }


        public bool Add(string sLog)
        {
            try
            {
                clearErrores();

                //verificamos si existe directorio
                if (!CreateDirectory()) return false;

                string nombreArchivo = GetNameFile();//obtenemos nombre de archivo del día
                string cadena = ""; //obtenemos el contenido del archivo


                cadena += DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss") + " - " + sLog + Environment.NewLine;

                //creamos el archivo y guardamos
                StreamWriter sw = new StreamWriter(FullPath + "/" + nombreArchivo, true);
                sw.Write(cadena);
                sw.Close();

                return true;

            }
            catch (DirectoryNotFoundException ex)
            {
                addError("Directorio invalido: " + FullPath);
                return false;
            }
            catch (FileNotFoundException ex)
            {
                addError("Ruta de archivo invalida");
                return false;
            }
            catch (Exception ex)
            {

                addError("Error: " + ex.Message);
                return false;
            }
        }



        #region Helpers

        private string GetNameFile()
        {
            string nombre = "";
            nombre = "Grupo3S_" + DateTime.Now.ToString("yyyyMMdd") + ".log";

            return nombre;
        }

        private bool CreateDirectory()
        {
            try
            {
                //si no existe el directorio lo creamos
                if (!Directory.Exists(FullPath))
                    Directory.CreateDirectory(FullPath);

                return true;

            }
            catch (DirectoryNotFoundException ex)
            {
                addError("Directorio invalido: " + FullPath);
                return false;
            }
        }


        public void addError(string error)
        {
            lErrores.Add(error);
        }

        public string getErrores()
        {
            string error = "";
            foreach (string err in lErrores)
            {
                error += err + BRINCO;
            }
            return error;
        }

        protected void clearErrores()
        {
            lErrores = new List<string>();
        }
        #endregion
    }
}