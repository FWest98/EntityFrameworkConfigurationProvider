using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EntityFrameworkConfigurationProvider {
    public class ConfigurationOption {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
