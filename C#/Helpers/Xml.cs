#region
using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
#endregion

namespace Xml.Process
{
    internal class Xml
    {
        private bool _isValid;
        private string Errors;

        public static XmlReader GetXmlReaderFromPath(string xsdPath)
        {
            return XmlReader.Create(new StreamReader(xsdPath));
        }

        #region Validation
        //public bool ValidateXml(byte[] xmlDocument, string xsdPath)
        //{
        //    return ValidateXml(new MemoryStream(xmlDocument), XmlReader.Create(new StreamReader(xsdPath)));
        //}

        public bool ValidateXml(Stream xmlDocument,string schemaName, XmlReader xsd, out string errors)
        {
            _isValid = true;
            Errors = string.Empty;
            var settings = new XmlReaderSettings();
            settings.Schemas.Add(schemaName, xsd);
            settings.ValidationEventHandler += MyValidationEventHandler;
            settings.ValidationType = ValidationType.Schema;

            var doc = new XmlDocument();

            //var document = XmlReader.Create(xmlDocument);
            //doc.Load(XmlReader.Create(document, settings));
            xmlDocument.Position = 0;
            doc.Load(XmlReader.Create(xmlDocument, settings));

            doc.Validate(MyValidationEventHandler);

            errors = Errors;

            return _isValid;
        }

        /// <summary>
        /// Callback de validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void MyValidationEventHandler(object sender, ValidationEventArgs args)
        {
            _isValid = false;
            Errors += args.Message + Environment.NewLine;
            //LogHelper.Log(this, LogHelper.ErrorLevel.ERROR, "Erreur de validation : " + args.Message);
        }
        #endregion

        #region Méthode de parcours du xml
        //public XmlNodeList FindNodes(byte[] xmlDocument, string xpath)
        //{
        //    return FindNodes(new MemoryStream(xmlDocument), xpath);
        //}

        public XmlNodeList FindNodesBySearch(Stream xmlDocument, string tagName)
        {
            xmlDocument.Position = 0;

            var xmldoc = new XmlDocument();

            //Load books.xml into the XmlDocument object. 
            xmldoc.Load(xmlDocument);
            return xmldoc.GetElementsByTagName(tagName);

        }

#warning Ne pas utiliser, valeur en dur!!!
        public XmlNodeList FindNodesByPath(Stream xmlDocument, string xpath)
        {
            xmlDocument.Position = 0;

            var xmldoc = new XmlDocument();

            //Load books.xml into the XmlDocument object. 
            xmldoc.Load(xmlDocument);

            var documentNamespace = xmldoc.DocumentElement.NamespaceURI;

            //Instantiate an XmlNamespaceManager object. 
            var xmlnsManager = new XmlNamespaceManager(xmldoc.NameTable);

            //Add the namespaces used in books.xml to the XmlNamespaceManager.
            xmlnsManager.AddNamespace("test", documentNamespace);

            //Execute the XPath query using the SelectNodes method of the XmlDocument.
            //Supply the XmlNamespaceManager as the nsmgr parameter.
            //The matching nodes will be returned as an XmlNodeList.
            //Use an XmlNode object to iterate through the returned XmlNodeList.

            XmlNodeList xmlNodeList = xmldoc.SelectNodes(xpath, xmlnsManager);

            //IEnumerator ienum = xmlNodeList.GetEnumerator();
            //while (ienum.MoveNext())
            //{
            //    XmlNode title = (XmlNode)ienum.Current;
            //    foreach (XmlAttribute node in title.Attributes)
            //    {
            //        Console.WriteLine(node.Value);
            //    }
            //}

            return xmlNodeList;
        }

        //public XmlNode FindNodeByPath(Stream xmlDocument, string xpath)
        //{
        //    var xmlNodeList = FindNodesByPath(xmlDocument, xpath);
        //    if (xmlNodeList.Count != 1)
        //    {
        //        throw new Exception("Error : Nombre de noeud inattendu!");
        //    }
        //    return xmlNodeList[0];
        //}

        public XmlNode FindNodeBySearch(Stream xmlDocument, string tagName)
        {
            var xmlNodeList = FindNodesBySearch(xmlDocument, tagName);
            if (xmlNodeList.Count != 1)
            {
                throw new Exception("Error : Nombre de noeud inattendu!");
            }
            return xmlNodeList[0];
        }

        //public string FindNodeDataByPath(Stream xmlDocument, string xpath)
        //{
        //    return FindNodeByPath(xmlDocument, xpath).InnerText;
        //}
        public string FindNodeDataBySearch(Stream xmlDocument, string tagName)
        {
            return FindNodeBySearch(xmlDocument, tagName).InnerText;
        }
        #endregion


#if NOT_ACTUALLY_USED
        public XPathNodeIterator FindNodes(byte[] xmlDocument, string xpath)
        {
            XmlTextReader reader = new XmlTextReader(new MemoryStream(xmlDocument));
            XPathDocument xPathDoc = new XPathDocument(reader);

            XPathNavigator xPathNav = xPathDoc.CreateNavigator();
            var xnm = new XmlNamespaceManager(reader.NameTable);
            xnm.AddNamespace("test", "my.xsd");

            //XPathExpression expr = xPathNav.Compile(xpath);
            XPathNodeIterator nodes = xPathNav.Select(xpath, xnm);

            while (nodes.MoveNext())
                Console.WriteLine(nodes.Current.Value);

            return nodes;
        }

//        public bool ValidateXml2()
//        {
//            var r = new XmlTextReader("C:\\Dossier\\ProductWithXSD.xml");
//            var v = new XmlValidatingReader(r);
//            _isValid = true;

//            v.ValidationType = ValidationType.Schema;

//            v.ValidationEventHandler +=
//                MyValidationEventHandler;

//            while (v.Read())
//            {
//                // Permet d'ajouter du code ici afin de traiter le contenu.
//            }
//            v.Close();
//// Permet de vérifier si le document est valide ou non.
//            if (_isValid)
//            {
//                Console.WriteLine(" Le document est valide ");
//            }
//            else
//            {
//                Console.WriteLine(" Le document n'est pas valide ");
//            }
//            return _isValid;
//        }
#endif

    }
}