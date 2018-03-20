using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGrupo3S.Helpers
{
    public class accesos
    {
        public string acc = "";
        public int BuscaPadre(string parRe, int opcion, string catalogo)
        {
            int dat = 0;
            if (parRe.ToUpper() == catalogo)
                dat = Convert.ToInt16(opcion);

            return dat;
        }

        public void CargaPerP(string parRe, string codpadre, ref int[] ma)
        {
            // 1 - INGRESO, 2 -  NUEVO, 3 - EDITAR, 4 - DEBAJA, 5 - RESETEA, 6 , 7, 8, 14 - REPORTE 
            
            if (Convert.ToInt16(codpadre) == ma[0])
            {
                ma[1] = 1; // SI EXISTE UNA OPCION ACTIVA SETEA POSITIVO EL INGRESO
                switch (parRe.ToUpper())
                {
                    case "NUEVO":
                        ma[2] = 1;
                        break;
                    case "EDITAR":
                        ma[3] = 1;
                        break;
                    case "DEBAJA":
                        ma[4] = 1;
                        break;
                    case "PERFILES":
                        ma[5] = 1;
                        break;
                    case "RESETEA":
                        ma[6] = 1;
                        break;
                    case "HORARIOS":
                        ma[7] = 1;
                        break;
                    case "DIRECCIONES":
                        ma[8] = 1;
                        break;
                    case "CONTACTOS":
                        ma[9] = 1;
                        break;
                    case "PERMISOS":
                        ma[10] = 1;
                        break;
                    case "DETALLE":
                        ma[11] = 1;
                        break;
                    case "REPORTE":
                        ma[19] = 1;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}