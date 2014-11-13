using System.Collections.Generic;
using System.Linq;

namespace MyApp.core
{
    public interface IApplicationSettings
    {
        List<string> ImplementedLanguages { get; }
        bool CreateContentOnAllLanguages { get; }
        bool ShouldGoToFirstMenuItem { get; }
        bool AllProtected { get; }
        string DefaultLanguage { get; }
        string LanguageConstraint { get; }
    }




    public class ProductionApplicationSettings : IApplicationSettings
    {
        private bool _allProtected;



        public List<string> ImplementedLanguages
        {
            get
            {
                return new List<string> { "ru", "en" };
            }
        }

        public bool CreateContentOnAllLanguages { get { return true; } }
        public bool ShouldGoToFirstMenuItem { get { return true; } }
        public bool AllProtected { get { return _allProtected; } }
        public string DefaultLanguage { get { return "ru"; } }
        public string LanguageConstraint
        {
            get
            {
                string res = ImplementedLanguages.Aggregate(string.Empty, (current, implementedLanguage) => current + ("(" + implementedLanguage + ")|"));
                if (res.Last() == '|')
                    res = res.Remove(res.Length - 1);
                return res;
            }
        }
    }


   
}
