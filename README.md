# MyMarsPlugin

<p align="center">
    <img src="assets/icon.png" alt="logo" />
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/mdimai666.Mars.Core">
    <img src="https://img.shields.io/nuget/v/mdimai666.Mars.Core" alt="nuget Version" />
  </a>
</p>

Шаблон для создания новых плагинов для [Mars](https://github.com/mdimai666/Mars)

## Быстрый старт

Выполните следующую команду в PowerShell для клонирования и автоматической настройки плагина:

```powershell
# Клонирование и настройка плагина. Используйте слово Plugin в конце
$newPluginName = "MyNewPlugin"; git clone https://github.com/mdimai666/MyMarsPlugin.git $newPluginName; cd $newPluginName; .\prepare.ps1 $newPluginName
```