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

## Структура

```
src/
  MyMarsPlugin/        # Бэкенд плагина (SDK: Microsoft.NET.Sdk.Razor, net10.0)
                       #   — точка входа, сервисы, имплементации нод, данные
  MyMarsPlugin.Front/  # Фронтенд (SDK: Microsoft.NET.Sdk.BlazorWebAssembly, net10.0)
                       #   — определения нод, формы редактирования, Startup фронта
  MyMarsPlugin.Shared/ # Общие DTO/опции/локализация (SDK: Microsoft.NET.Sdk, net10.0)
```

## Быстрый старт

Выполните следующую команду в PowerShell для клонирования и автоматической настройки плагина:

```powershell
# Клонирование и настройка плагина. Используйте слово Plugin в конце
$newPluginName = "MyNewPlugin"; git clone https://github.com/mdimai666/MyMarsPlugin.git $newPluginName; cd $newPluginName; .\prepare.ps1 $newPluginName
```

`prepare.ps1` переименовывает проекты/файлы/содержимое (включая `.github` и `release.ps1`)
и удаляет служебные файлы шаблона. После этого обновите `PackageId`, `Description`,
`RepositoryUrl` (и при желании `Product`/`Title`) в `src/<Имя>/<Имя>.csproj`, `Authors`/`Company`
в корневом `Directory.Build.props` и `assets/icon.png`.

## Версии — единый источник в Directory.Build.props

В корне репозитория лежит `Directory.Build.props` (аналог одноимённого файла в репозитории
Mars). Он раздаёт всем проектам:

- `PluginVersion` — версия плагина (`<Version>`, тег релиза `v<PluginVersion>`);
- `MarsVersion` — версия пакетов Марса, на неё ссылаются `Version="$(MarsVersion)"`
  в csproj (`Mars.Plugin.Sdk`, `Mars.Plugin.Kit.*`, `Mars.Data.*`);
- общую метадату пакета: `Authors`, `Company`, `PackageTags`, `PackageIcon`,
  лицензию MIT, `Microsoft.SourceLink.GitHub`.

Проектно-специфичное (`PackageId`, `Description`, `RepositoryUrl`) остаётся в основном csproj.
`TargetFramework`/`Nullable`/`ImplicitUsings` тоже заданы в props — в csproj их держать не нужно.

## Паковка плагина

Паковку выполняет пакет `mdimai666.Mars.Plugin.Sdk` (подключён в основном проекте с
`PrivateAssets="all"`) — свои MSBuild-таргеты в csproj плагина писать **не нужно**.
Метаданные пакета берутся из корневого `Directory.Build.props` (`Version` из
`PluginVersion`, `Authors`, `PackageTags`, `PackageIcon`) и из csproj основного проекта
(`PackageId`, `Title`, `Description`, `RepositoryUrl`).

### Zip — установка через админку

```powershell
dotnet publish src/MyMarsPlugin/MyMarsPlugin.csproj -c Release
```

После публикации автоматически срабатывает `MarsPluginPackZip`: отсекает сборки Марса
из вывода (по `Mars.deps.json`), пишет дескриптор `mars-plugin.json` и собирает
`<PackageId>-<Version>.zip` в `bin/Release/net10.0/`. Zip самокомплектный — в него
попадают и сторонние библиотеки, которых нет в Марсе.

### NuGet — распространение через фид/маркетплейс

```powershell
dotnet msbuild src/MyMarsPlugin/MyMarsPlugin.csproj -t:MarsPluginPackNuget -p:Configuration=Release
```

Собирает `<PackageId>.<Version>.nupkg` рядом с папкой публикации, классический лейаут:

```
lib/net10.0/            # сборки плагина
mars/front/             # фронт-ассеты
mars/mars-plugin.json   # дескриптор (entry, MarsVersion и т.д.)
icon.png
```

Зависимости (включая пакеты Марса) объявляются в nuspec; `<packageType>MarsPlugin</packageType>`
позволяет фильтровать плагины на фиде. Сторонние библиотеки в файлы пакета не кладутся —
при установке Марс сам резолвит замыкание и раскладывает в `data/plugins/<PackageId>/`
только то, чего нет в его замыкании.

### Дескриптор mars-plugin.json

Пишется автоматически: `PackageId`, `Version`, `EntryAssembly` (по конвенции
`<ИмяПроекта>.dll`), `MarsVersion` — версия Марса, инструментом которого упакован плагин.
Рантайм читает только его.

### Установка в Mars

- **Zip** — админка `Plugins` → «Upload from zip-file».
- **NuGet** — админка «Install from NuGet» по `PackageId` (или `Mars.exe plugin install`).
- Все изменения (установка/отключение/удаление/обновление) применяются **после рестарта**
  сервера; каждый плагин грузится в собственный `AssemblyLoadContext` — версии сторонних
  библиотек не конфликтуют, сборки Марса резолвятся из хоста.

## Публикация на nuget.org

Релиз = тег `v<PluginVersion>` (версия из корневого `Directory.Build.props` — единый
источник; nuget-пакет собирается один — из основного проекта, под-проекты отдельно
не публикуются). Пуш тега запускает `.github/workflows/nuget-publish.yml`, который пакует
плагин (`MarsPluginPackNuget`) и публикует на nuget.org через Trusted Publishing
(OIDC, без API-ключей).

```powershell
# 1. Поднимите PluginVersion в Directory.Build.props и закоммитьте
# 2. Запустите скрипт — он проверит ветку/тег/опубликованность и запушит тег
.\release.ps1        # с подтверждениями
.\release.ps1 -y     # без подтверждений
```

Настройка Trusted Publishing (один раз, для каждого репозитория плагина):
1. В секретах репозитория GitHub добавьте `NUGET_USER` — имя профиля nuget.org (не email).
2. На nuget.org: учётная запись → **Trusted Publishing** → добавьте политику:
   owner (владелец пакета), репозиторий GitHub `<владелец>/<репозиторий>`, workflow-файл
   `nuget-publish.yml`.
3. Убедитесь, что владелец пакета в nuget.org совпадает с автором политики.

## Что необходимо знать при разработке

**Версии пакетов.** Версия плагина — `PluginVersion` в корневом `Directory.Build.props`;
версия пакетов Марса (`Mars.Plugin.Sdk`, `Mars.Plugin.Kit.Host`, `Mars.Plugin.Kit.Front`,
`Mars.Data.PostgreSQL`) — `MarsVersion` там же, выравнивайте её с текущим релизом Марса
(`MarsAppVersion` в `Directory.Build.props` репозитория Mars). Плагин пакуется против
версии платформы, указанной в `Mars.Plugin.Sdk`.

**Регистрация плагина.** Бэкенд: `[assembly: MarsPlugin(typeof(MainXxxPlugin))]` + класс
`MainXxxPlugin : MarsPlugin`; в `ConfigureWebApplicationBuilder` регистрируйте сервисы, в
`ConfigureWebApplication` вызывайте
`AutoHostRegisterHelper([GetType().Assembly, typeof(<ФронтовыйТип>).Assembly])` — так
регистрируются имплементации нод. Фронт: `IWebAssemblyPluginFront` +
`AutoFrontRegisterHelper([GetType().Assembly])`.

**Ноды.** Определение (`Node`/`ConfigNode`) живёт во Front-проекте, имплементация
(`INodeImplement<T>`) — в основном. В конструкторе имплементации всегда выполняйте
`Node.Config = RNS.GetConfig(node.Config)`. Продвижение результата: `callback(input)`
или `callback(input, outputIndex)`. Статус/отладка: `RNS.Status(...)`, `RNS.DebugMsg(...)`.

**Формы.** Компонент `NodeEditForm` с атрибутом `[NodeEditFormForNode(typeof(...))]`
во Front-проекте; поля — через `FormItem2`/`InputConfigField`/`FluentTextField`.

**Данные и БД.** Плагин может объявлять свои EF-сущности и миграции через
`IPluginDatabaseMigrator` (см. примеры в каталоге плагинов Mars).

**Где подробнее:**
- Создание плагина: `ai/PluginCreationGuide.md` в репозитории Mars;
- Упаковка: `docs/dev_docs/Plugins/PluginSdk.md`;
- Старт: `docs/dev_docs/Plugins/PluginGettingStart.md`;
- XActions (команды платформы): `docs/dev_docs/Plugins/XActions.md`;
- Примеры плагинов: [YandexWeatherPlugin](https://github.com/mdimai666/YandexWeatherPlugin),
  [Mars.TelegramPlugin](https://github.com/mdimai666/Mars.TelegramPlugin),
  [Mars.PlayAudioNodePlugin](https://github.com/mdimai666/Mars.PlayAudioNodePlugin).
