using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocumentCreator
{
    public abstract class Document
    {
        public DocumentOperator Operator { get; set; }
        protected MemoryStream _stream;
        private string _fileName;
        protected List<int> _password;
        protected DateTime _dateCreated;
        public Document()
        {
            _dateCreated = DateTime.Now;
            _password = new List<int>();
            var rnd = new Random();
            for (var i = 1; i <= 4; i++)
            {
                _password.Add(rnd.Next(2, 9));
            }
            _stream = new MemoryStream();
        }
        public byte[] ToArray()
        {
            return _stream.ToArray();
        }
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = Path.GetInvalidFileNameChars().Aggregate(value.Trim(), (f, c) => f.Replace(c.ToString(), "_"));
            }
        }
    }
    public class DocumentOperator
    {
        public enum Operators
        {
            Ionia,
            Grace
        }
        private readonly string _contentPath;
        public Operators Operator { get; private set; }
        public DocumentOperator(Operators op, string contentPath)
        {
            _contentPath = contentPath;
            Operator = op;
        }
        public FileStream? GetOperatorImageStream()
        {
            if (!File.Exists(FilePath))
            {
                return null;
            }
            var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            if (fs.Length == 0) return null;
            return fs;
        }

        public string FilePath
        {
            get
            {
                return Path.Combine(_contentPath, FileName);
            }
        }
        public string FileName
        {
            get
            {
                return Operator == Operators.Ionia ? "ionia.png" : "grace.jpg";
            }
        }
        public string EmailDomain
        {
            get
            {
                return Operator == Operators.Ionia ? "ioniaman.gr" : "graceshipman.gr";
            }
        }
        public string Department { get; set; }
        public string Email
        {
            get
            {
                if (string.IsNullOrEmpty(Department))
                {
                    return null;
                }
                return Department + "@" + EmailDomain;
            }
        }
        public string Title
        {
            get
            {
                return Operator == Operators.Ionia ? "IONIA MANAGEMENT S.A." : "GRACE MANAGEMENT S.A.";
            }
        }
        public string Address
        {
            get
            {
                return Operator == Operators.Ionia ? "1 Dervenakion str. 18545, Piraeus Greece, Tel: +30 210 4065000, Fax: +30 210 4065001" : "1 Dervenakion str. 18545, Piraeus Greece, Tel: +30 210 40 65 250, Fax: +30 210 40 65 251";
            }
        }
    }
}
