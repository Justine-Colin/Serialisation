using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Serialisation
{
    class Program
    {
        static void Main(string[] args)
        {
            #region PersonneSerialisee
            PersonneSerialisee ps = new PersonneSerialisee(1, "Winch", "Largo", new DateTime(1975, 07, 15));
            ps.Lst.Add("Danitza");
            ps.Lst.Add("Charity");
            ps.Lst.Add("Marilyn");
            ps.SerialiserFichier("essai.xml");
            PersonneSerialisee psBis = PersonneSerialisee.DeserialiserFichier("essai.xml");
            Console.WriteLine("{0} {1} ({2})", psBis.Prenom, psBis.Nom, psBis.ID);
            #endregion
            #region Personne
            Personne p = new Personne(1, "Winch", "Largo", new DateTime(1975, 07, 15));
            p.Lst.Add("Danitza");
            p.Lst.Add("Charity");
            p.Lst.Add("Marilyn");
            p.SerialiserFichier("essaibis.xml");
            Personne pBis = Personne.DeserialiserFichier("essaibis.xml");
            Console.WriteLine("{0} {1} ({2})", pBis.Prenom, pBis.Nom, pBis.ID);
            p.SerialiserToutFichier("essaiter.xml");
            Personne pTer = Personne.DeserialiserToutFichier("essaiter.xml");
            Console.WriteLine("{0} {1} ({2})", pTer.Prenom, pTer.Nom, pTer.ID);
            #endregion
            #region Générique
            UtilitaireSerialisation.SerialiserFichier<Personne>("essai4.xml", p);
            Personne pQuat = UtilitaireSerialisation.DeserialiserFichier<Personne>("essai4.xml");
            Console.WriteLine("{0} {1} ({2})", pQuat.Prenom, pQuat.Nom, pQuat.ID);
            List<Personne> LPer = new List<Personne>();
            LPer.Add(p);
            LPer.Add(pBis);
            LPer.Add(pTer);
            LPer.Add(pQuat);
            UtilitaireSerialisation.SerialiserFichier<List<Personne>>("essai5.xml", LPer);
            List<Personne> Lp2 = UtilitaireSerialisation.DeserialiserFichier<List<Personne>>("essai5.xml");
            foreach (var v in Lp2)
                Console.WriteLine("{0} {1} ({2})", v.Prenom, v.Nom, v.ID);
            #endregion
            Console.ReadLine();
        }
    }

    [Serializable]
    [XmlRoot()]
    public class PersonneSerialisee
    {
        [XmlAttribute("Identifiant")]
        public int ID { get; set; }
        [XmlElement("Nom")]
        public string Nom { get; set; }
        [XmlElement("Prenom")]
        public string Prenom { get; set; }
        [XmlIgnore]
        public DateTime Naissance { get; set; }
        [XmlArray("Liste")]
        [XmlArrayItem("Conquête")]
        public List<string> Lst { get; set; }
        public PersonneSerialisee()
        { Lst = new List<string>();  }
        public PersonneSerialisee(int ID_, string Nom_, string Prenom_, DateTime Naissance_) : this()
        {
            ID = ID_;
            Nom = Nom_;
            Prenom = Prenom_;
            Naissance = Naissance_;
        }
        public void SerialiserFichier(string nf)
        {
            using (StreamWriter sw = new StreamWriter(nf))
            {
                XmlSerializer xs = new XmlSerializer(this.GetType());
                xs.Serialize(sw, this);
                sw.Close();
            }
        }
        public static PersonneSerialisee DeserialiserFichier(string nf)
        {
            using (StreamReader sr = new StreamReader(nf))
            {
                XmlSerializer xs = new XmlSerializer(typeof(PersonneSerialisee));
                PersonneSerialisee rep = (PersonneSerialisee)xs.Deserialize(sr);
                sr.Close();
                return rep;
            }
        }
    }

    public class Personne
    {
        public int ID { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public DateTime Naissance { get; set; }
        public List<string> Lst { get; set; }
        public Personne()
        { Lst = new List<string>(); }
        public Personne(int ID_, string Nom_, string Prenom_, DateTime Naissance_) : this()
        {
            ID = ID_;
            Nom = Nom_;
            Prenom = Prenom_;
            Naissance = Naissance_;
        }
        public void SerialiserFichier(string nf)
        {
            using (XmlTextWriter xw = new XmlTextWriter(nf, Encoding.UTF8))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("Personne");
                xw.WriteAttributeString("Identifiant", ID.ToString());
                xw.WriteElementString("Prenom", Prenom);
                xw.WriteElementString("Nom", Nom);
                xw.WriteStartElement("Liste");
                foreach (string l in Lst)
                    xw.WriteElementString("Conquête", l);
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndDocument();
                xw.Close();
            }
        }
        public static Personne DeserialiserFichier(string nf)
        {
            Personne rep = new Personne();
            XmlTextReader xr = new XmlTextReader(nf);
            while(xr.Read())
            {
                if(xr.Name == "Personne")
                {
                    xr.MoveToAttribute("Identifiant");
                    rep.ID = xr.ReadContentAsInt();
                    xr.Read();
                    rep.Prenom = xr.ReadElementContentAsString();
                    rep.Nom = xr.ReadElementContentAsString();
                    if (xr.Name == "Liste" && !xr.IsEmptyElement)
                    {
                        xr.Read();
                        while (xr.Name == "Conquête")
                            rep.Lst.Add(xr.ReadElementContentAsString());
                    }
                    xr.Read();
                }
            }
            return rep;
        }
        public void SerialiserToutFichier(string nf)
        {
            XmlSerializer xs = new XmlSerializer(this.GetType());
            StreamWriter sw = new StreamWriter(nf);
            xs.Serialize(sw, this);
            sw.Close();
        }

        public static Personne DeserialiserToutFichier(string nf)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Personne));
            StreamReader sr = new StreamReader(nf);
            Personne rep = (Personne)xs.Deserialize(sr);
            sr.Close();
            return rep;
        }
    }

    public class UtilitaireSerialisation
    {
        public static void SerialiserFichier<T>(string nf, T obj)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            StreamWriter sw = new StreamWriter(nf);
            xs.Serialize(sw, obj);
            sw.Close();
        }

        public static T DeserialiserFichier<T>(string nf)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            StreamReader sr = new StreamReader(nf);
            T rep = (T)xs.Deserialize(sr);
            sr.Close();
            return rep;
        }
    }
}
