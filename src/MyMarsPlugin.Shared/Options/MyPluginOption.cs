using System.ComponentModel.DataAnnotations;

namespace MyMarsPlugin.Shared.Options;

[Display(Name = "Настройки моего плагина")]
public class MyPluginOption
{
    [Required]
    [Display(Name = "Option value", Description = "some value description")]
    public string OptionValue { get; set; } = "default1";
}
