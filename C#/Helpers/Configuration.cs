using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace Helper.Configuration
{
    /// <summary>
    /// Lecture de paramètres dans le fichier de configuration de l'application
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Lecture d'un paramètre de type chaine dans le fichier de configuration de l'application
        /// </summary>
        /// <param name="param">nom du paramètre à lire</param>
        /// <returns>la chaine correspondant au paramètre</returns>
        public static string ReadString(string param)
        {
            return System.Configuration.ConfigurationManager.AppSettings[param];
        }

        /// <summary>
        /// Lecture d'un paramètre de type entier dans le fichier de configuration de l'application
        /// </summary>
        /// <param name="param">nom du paramètre à lire</param>
        /// <returns>l'entier correspondant au paramètre</returns>
        public static int ReadInt(string param)
        {
            return int.Parse(System.Configuration.ConfigurationManager.AppSettings[param]);
        }

        /// <summary>
        /// Lecture d'un paramètre de type chemin d'un répertoire dans le fichier de configuration de l'application
        /// (et vérification de l'existance du répertoire)
        /// </summary>
        /// <param name="param">nom du paramètre à lire</param>
        /// <param name="oldValue">valeur (éventuelle) à remplacer </param>
        /// <param name="newValue">valeur (éventuelle) de remplacement</param>
        /// <param name="verifyPath">true si l'existance du répertoire doit être vérifié
        /// false sinon</param>
        /// <returns>le chemin du répertoire (contenant un '\' final) correspondant au paramètre</returns>
        public static string ReadPath(string param, string oldValue, string newValue, bool verifyPath)
        {
            var path = ReadString(param);
            if (string.IsNullOrEmpty(path))
            {
                throw new ConfigurationErrorsException("le chemin est de longueur nulle.");
            }
            if (!string.IsNullOrEmpty(oldValue))
            {
                path = path.Replace(oldValue, newValue);
            }
            if (verifyPath && !Directory.Exists(path))
            {
                throw new ConfigurationErrorsException(string.Format("le chemin {0} n'existe pas", path));
            }
            return FileManagment.FormatPath(path);
        }

        /// <summary>
        /// Lecture d'un paramètre de type chemin d'un répertoire dans le fichier de configuration de l'application
        /// (et vérification de l'existance du répertoire)
        /// </summary>
        /// <param name="param">nom du paramètre à lire</param>
        /// <returns>le chemin du répertoire (contenant un '\' final) correspondant au paramètre</returns>
        public static string ReadAndVerifyPath(string param)
        {
            return ReadPath(param, null, null, true);
        }
    }
}