using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gvss_project
{
    public static class DefaultSettings {

        private static LanguageMode lanMode;

        public static readonly Dictionary<OperatorLevel, (double, double)> OperatorCoefs = new Dictionary<OperatorLevel, (double, double)> {
        {OperatorLevel.Second, (0.69,0.18) },
        {OperatorLevel.Third, (0.58,0.09) },
        {OperatorLevel.Forth, (0.46,0.03) },
        {OperatorLevel.Fifth, (0.42,0.025) }
        };
        public static readonly Dictionary<(SensorRequestBodies, SensorRequestPlacementMethod), double> RequestTime = new Dictionary<(SensorRequestBodies, SensorRequestPlacementMethod), double>() {
            { (SensorRequestBodies.ToggleSwitch, SensorRequestPlacementMethod.MnemonicDiagram), 0.2},
            { (SensorRequestBodies.ToggleSwitch, SensorRequestPlacementMethod.GroupMethod), 0.24},
            { (SensorRequestBodies.ToggleSwitch, SensorRequestPlacementMethod.BlockDiagramMethod), 0.15},
            { (SensorRequestBodies.ToggleSwitch, SensorRequestPlacementMethod.UnorganizedMethod), 0.3},
            { (SensorRequestBodies.Button, SensorRequestPlacementMethod.MnemonicDiagram), 0.12},
            { (SensorRequestBodies.Button, SensorRequestPlacementMethod.GroupMethod), 0.16},
            { (SensorRequestBodies.Button, SensorRequestPlacementMethod.BlockDiagramMethod), 0.07},
            { (SensorRequestBodies.Button, SensorRequestPlacementMethod.UnorganizedMethod), 0.23},
            { (SensorRequestBodies.Dialer, SensorRequestPlacementMethod.Separately), 1.8},
            { (SensorRequestBodies.KeyboardDevice, SensorRequestPlacementMethod.Separately), 0.15},
            { (SensorRequestBodies.Dialer, SensorRequestPlacementMethod.GroupMethod), 1.8},
            { (SensorRequestBodies.KeyboardDevice, SensorRequestPlacementMethod.GroupMethod), 0.15}
        };
        public static readonly Dictionary<SensorControlAgent, double> ControlAgentsTime = new Dictionary<SensorControlAgent, double>() {
            { SensorControlAgent.SignalingDevice, 0},
            { SensorControlAgent.WeakDampedDevice, 1},
            { SensorControlAgent.HighlyDampedDevice, 0.4},
            { SensorControlAgent.DigitalThreeDigitIndicator, 0.35}
        };
        public static readonly Dictionary<InformationRepresentationMethod, double> IndformationRepresentationMethodCoefs = new Dictionary<InformationRepresentationMethod, double>() {
            { InformationRepresentationMethod.MnemonicDiagram, 1},
            { InformationRepresentationMethod.GroupMethod, 0.85},
            { InformationRepresentationMethod.BlockDiagramMethod, 1.2}
        };

        private static readonly Dictionary<(LanguageMode, Labels), string> LabelsText = new Dictionary<(LanguageMode, Labels), string>() {
            { (LanguageMode.Russian, Labels.LanguageMode), "Язык"},
            { (LanguageMode.Ukrainian, Labels.LanguageMode), "Мова"},

            { (LanguageMode.Russian, Labels.RegisterAuxiliaryParam), "Зарегистрировать признак"},
            { (LanguageMode.Ukrainian, Labels.RegisterAuxiliaryParam), "Зареєструвати ознаку"},

            { (LanguageMode.Russian, Labels.Letter), "Буква"},
            { (LanguageMode.Ukrainian, Labels.Letter), "Літера"},

            { (LanguageMode.Russian, Labels.Index), "Индекс"},
            { (LanguageMode.Ukrainian, Labels.Index), "Індекс"},

            { (LanguageMode.Russian, Labels.ControlMethod), "Способ контроля"},
            { (LanguageMode.Ukrainian, Labels.ControlMethod), "Спосіб контролю"},

            { (LanguageMode.Russian, Labels.ControlAgentAuto), "Средство контроля параметров"},
            { (LanguageMode.Ukrainian, Labels.ControlAgentAuto), "Засіб контролю параметрів"},

            { (LanguageMode.Russian, Labels.ControlAgentRequest), "Средство запроса (прибор)"},
            { (LanguageMode.Ukrainian, Labels.ControlAgentRequest), "Засіб запиту (пристрій)"},

            { (LanguageMode.Russian, Labels.RequestBody), "Орган запроса"},
            { (LanguageMode.Ukrainian, Labels.RequestBody), "Орган запиту"},

            { (LanguageMode.Russian, Labels.RequestPlacementMethod), "Размещение органа запроса"},
            { (LanguageMode.Ukrainian, Labels.RequestPlacementMethod), "Розміщення органу запиту"},

            { (LanguageMode.Russian, Labels.Description), "Краткое описание"},
            { (LanguageMode.Ukrainian, Labels.Description), "Стислий опис"},

            { (LanguageMode.Russian, Labels.Managment), "Управление"},
            { (LanguageMode.Ukrainian, Labels.Managment), "Управління"},

            { (LanguageMode.Russian, Labels.AuxiliaryParamAndViolation), "Признак/нарушение"},
            { (LanguageMode.Ukrainian, Labels.AuxiliaryParamAndViolation), "Ознака/порушення"},

            { (LanguageMode.Russian, Labels.GVSSSettings), "Настройки ГВСС"},
            { (LanguageMode.Ukrainian, Labels.GVSSSettings), "Налаштування ГВСС"},

            { (LanguageMode.Russian, Labels.NumOfTechLines), "Количество техн. линий"},
            { (LanguageMode.Ukrainian, Labels.NumOfTechLines), "Кількість техн. ліній"},

            { (LanguageMode.Russian, Labels.OperatorLevel), "Уровень тренированности оператора"},
            { (LanguageMode.Ukrainian, Labels.OperatorLevel), "Рівень тренованості оператора"},

            { (LanguageMode.Russian, Labels.InformationRepresentationMethod), "Метод представления информации"},
            { (LanguageMode.Ukrainian, Labels.InformationRepresentationMethod), "Спосіб представлення інформації"},

            { (LanguageMode.Russian, Labels.Event), "Событие"},
            { (LanguageMode.Ukrainian, Labels.Event), "Подія"},

            { (LanguageMode.Russian, Labels.ChooseEvent), "Выберите событие"},
            { (LanguageMode.Ukrainian, Labels.ChooseEvent), "Оберіть подію"},

            { (LanguageMode.Russian, Labels.InformationQuantity), "Количество информации"},
            { (LanguageMode.Ukrainian, Labels.InformationQuantity), "Кількість інформації"},

            { (LanguageMode.Russian, Labels.OriginalTime), "Исходное время"},
            { (LanguageMode.Ukrainian, Labels.OriginalTime), "Початковий час"},

            { (LanguageMode.Russian, Labels.RequestTime), "Время потр. на запросы"},
            { (LanguageMode.Ukrainian, Labels.RequestTime), "Час, витрачений на запити"},

            { (LanguageMode.Russian, Labels.IrrelevantInformationTime), "Задержка (ир. информ.)"},
            { (LanguageMode.Ukrainian, Labels.IrrelevantInformationTime), "Затримка (ір. інформ.)"},

            { (LanguageMode.Russian, Labels.AgentTimeInfluence), "Влияние средств предст."},
            { (LanguageMode.Ukrainian, Labels.AgentTimeInfluence), "Вплив засобів предст."},

            { (LanguageMode.Russian, Labels.FindControlAgentTime), "Поиск средств предст."},
            { (LanguageMode.Ukrainian, Labels.FindControlAgentTime), "Пошук засобів предст."},

            { (LanguageMode.Russian, Labels.TotalTime), "Общее время обнаружения события"},
            { (LanguageMode.Ukrainian, Labels.TotalTime), "Загальний час виявлення події"},

            { (LanguageMode.Russian, Labels.AddViolation), "Добавить нарушение"},
            { (LanguageMode.Ukrainian, Labels.AddViolation), "Додати порушення"},

            { (LanguageMode.Russian, Labels.Violation), "Нарушение"},
            { (LanguageMode.Ukrainian, Labels.Violation), "Порушення"},

            { (LanguageMode.Russian, Labels.ChooseAction), "Выберите действие"},
            { (LanguageMode.Ukrainian, Labels.ChooseAction), "Оберіть дію"},

            { (LanguageMode.Russian, Labels.FailureRate), "Частота выхода из строя"},
            { (LanguageMode.Ukrainian, Labels.FailureRate), "Частота виходу з ладу"},

            { (LanguageMode.Russian, Labels.AuxiliaryParam), "Признак"},
            { (LanguageMode.Ukrainian, Labels.AuxiliaryParam), "Ознака"},

            { (LanguageMode.Russian, Labels.AddAuxiliaryParamToVertex), "Добавить признак к вершине"},
            { (LanguageMode.Ukrainian, Labels.AddAuxiliaryParamToVertex), "Додати ознаку до вершини"},
        };
        private static readonly Dictionary<(LanguageMode, Buttons), string> ButtonsText = new Dictionary<(LanguageMode, Buttons), string>() {

            { (LanguageMode.Russian, Buttons.Register), "Зарегистрировать"},
            { (LanguageMode.Ukrainian, Buttons.Register), "Зареєструвати"},

            { (LanguageMode.Russian, Buttons.Edit), "Редактировать"},
            { (LanguageMode.Ukrainian, Buttons.Edit), "Редагувати"},

            { (LanguageMode.Russian, Buttons.Delete), "Удалить"},
            { (LanguageMode.Ukrainian, Buttons.Delete), "Видалити"},

            { (LanguageMode.Russian, Buttons.GetTime), "Получить время обнаружения"},
            { (LanguageMode.Ukrainian, Buttons.GetTime), "Отримати час виявлення"},

            { (LanguageMode.Russian, Buttons.Add), "Добавить"},
            { (LanguageMode.Ukrainian, Buttons.Add), "Додати"},

            { (LanguageMode.Russian, Buttons.Change), "Изменить"},
            { (LanguageMode.Ukrainian, Buttons.Change), "Змінити"}
        };
        private static readonly Dictionary<(LanguageMode, ToolTips), string> ToolTipsText = new Dictionary<(LanguageMode, ToolTips), string>() {

            { (LanguageMode.Russian, ToolTips.CreateNewGVSS), "Создать новую ГВСС"},
            { (LanguageMode.Ukrainian, ToolTips.CreateNewGVSS), "Створити нову ГВСС"},

            { (LanguageMode.Russian, ToolTips.LoadGVSS), "Загрузить"},
            { (LanguageMode.Ukrainian, ToolTips.LoadGVSS), "Завантажити"},

            { (LanguageMode.Russian, ToolTips.SaveGVSS), "Сохранить"},
            { (LanguageMode.Ukrainian, ToolTips.SaveGVSS), "Зберегти"},

            { (LanguageMode.Russian, ToolTips.SavePhoto), "Сохранить фото структуры"},
            { (LanguageMode.Ukrainian, ToolTips.SavePhoto), "Зберегти фото структури"},

            { (LanguageMode.Russian, ToolTips.ViolationsAreUnique), "Нарушения - уникальны. Регистрируются отдельно"},
            { (LanguageMode.Ukrainian, ToolTips.ViolationsAreUnique), "Порушення - унікальні. Реєструються окремо"},

            { (LanguageMode.Russian, ToolTips.ViolationsCanBeParams), "Нарушения могут быть признаками других нарушений"},
            { (LanguageMode.Ukrainian, ToolTips.ViolationsCanBeParams), "Порушення можуть бути ознаками інших порушень"}
        };

        private static readonly Dictionary<(LanguageMode, ErrorMessages), string> ErrorMessagesText = new Dictionary<(LanguageMode, ErrorMessages), string>() {

            { (LanguageMode.Russian, ErrorMessages.WhileAddRepetative), "Невозможно добавить признак в это поддерево"},
            { (LanguageMode.Ukrainian, ErrorMessages.WhileAddRepetative), "Неможливо додати ознаку в це піддерево"},

            { (LanguageMode.Russian, ErrorMessages.WhileASVFileCorrupted), "Файл .avs поврежден или внутренняя структура нарушена"},
            { (LanguageMode.Ukrainian, ErrorMessages.WhileASVFileCorrupted), "Файл .avs пошкоджений або внутрішня структура порушена"},

            { (LanguageMode.Russian, ErrorMessages.WhileGVSSFileCorrupted), "Файл .gvss поврежден или внутренняя структура нарушена"},
            { (LanguageMode.Ukrainian, ErrorMessages.WhileGVSSFileCorrupted), "Файл .gvvs пошкоджений або внутрішня структура порушена"},

            { (LanguageMode.Russian, ErrorMessages.TheNameExistsAlready), "* имя недоступно"},
            { (LanguageMode.Ukrainian, ErrorMessages.TheNameExistsAlready), "* ім'я вже зайняте"},

            { (LanguageMode.Russian, ErrorMessages.NotAllFieldsAreFilled), "* не все поля заполненны"},
            { (LanguageMode.Ukrainian, ErrorMessages.NotAllFieldsAreFilled), "* не всі поля заповнені"}

        };
        private static readonly Dictionary<(LanguageMode, MessageBoxMessages), string> MessageBoxText = new Dictionary<(LanguageMode, MessageBoxMessages), string>() {

            { (LanguageMode.Russian, MessageBoxMessages.ChangeMode), "Изменение режима может привести к удалению некоторых вершин. Продолжить?"},
            { (LanguageMode.Ukrainian, MessageBoxMessages.ChangeMode), "Зміна режиму може привести до видалення деяких вершин. Продовжити?"},

            { (LanguageMode.Russian, MessageBoxMessages.DeleteVertex), "Удаление этой вершины приведет к удалению всех дочерних вершин. Продолжить?"},
            { (LanguageMode.Ukrainian, MessageBoxMessages.DeleteVertex), "Видалення цієї вершини призведе до видалення всіх дочірніх вершин. Продовжити?"},

            { (LanguageMode.Russian, MessageBoxMessages.DoYouWantToSaveGVSSBeforeQuit), "Хотите ли вы сохранить эту ГВСС перед выходом?"},
            { (LanguageMode.Ukrainian, MessageBoxMessages.DoYouWantToSaveGVSSBeforeQuit), "Чи бажаєте ви зберегти цю ГВСС перед виходом?"},

            { (LanguageMode.Russian, MessageBoxMessages.FullDeleteParam), "Все вершины и признаки с таким уникальным кодом будут удалены. Продолжить?"},
            { (LanguageMode.Ukrainian, MessageBoxMessages.FullDeleteParam), "Всі вершини і ознаки з таким унікальним кодом будуть видалені. Продовжити?"},

            { (LanguageMode.Russian, MessageBoxMessages.SaveGVSSBeforeLoad), "Хотите ли вы сохранить эту ГВСС перед загрузкой?"},
            { (LanguageMode.Ukrainian, MessageBoxMessages.SaveGVSSBeforeLoad), "Чи бажаєте ви зберегти цю ГВСС перед завантаженням?"}
        };
        private static readonly Dictionary<(LanguageMode, ContextMenuHeaders), string> ContextMenuHeadersText = new Dictionary<(LanguageMode, ContextMenuHeaders), string>() {

            { (LanguageMode.Russian, ContextMenuHeaders.DeleteVertex), "Удалить вершину"},
            { (LanguageMode.Ukrainian, ContextMenuHeaders.DeleteVertex), "Видалити вершину"},

            { (LanguageMode.Russian, ContextMenuHeaders.Edit), "Редактировать"},
            { (LanguageMode.Ukrainian, ContextMenuHeaders.Edit), "Редагувати"}
        };
        private static readonly Dictionary<(LanguageMode, WindowTitles), string> WindowsTitlesText = new Dictionary<(LanguageMode, WindowTitles), string>() {

            { (LanguageMode.Russian, WindowTitles.Add), "Добавить"},
            { (LanguageMode.Ukrainian, WindowTitles.Add), "Додати"},

            { (LanguageMode.Russian, WindowTitles.Edit), "Редактировать"},
            { (LanguageMode.Ukrainian, WindowTitles.Edit), "Редагувати"},

            { (LanguageMode.Russian, WindowTitles.ErrorMB), "Ошибка"},
            { (LanguageMode.Ukrainian, WindowTitles.ErrorMB), "Помилка"},

            { (LanguageMode.Russian, WindowTitles.EventDetectionTime), "Время обнаружения события"},
            { (LanguageMode.Ukrainian, WindowTitles.EventDetectionTime), "Час розпізнавання події"},

            { (LanguageMode.Russian, WindowTitles.WarningMB), "Предупреждение"},
            { (LanguageMode.Ukrainian, WindowTitles.WarningMB), "Попередження"},

            { (LanguageMode.Russian, WindowTitles.MathLABResults), "MathLAB результаты"},
            { (LanguageMode.Ukrainian, WindowTitles.MathLABResults), "MathLAB результати"}
        };

        private static readonly Dictionary<(LanguageMode, SensorControlMethod), string> ControlMethodsText = new Dictionary<(LanguageMode, SensorControlMethod), string>(){

            { (LanguageMode.Russian, SensorControlMethod.Auto), "Автоматическое предъявление" },
            { (LanguageMode.Ukrainian, SensorControlMethod.Auto), "Автоматичне пред'явлення" },

            { (LanguageMode.Russian, SensorControlMethod.Request), "Запрос значений"},
            { (LanguageMode.Ukrainian, SensorControlMethod.Request), "Запит значень"}

            /*{ SensorControlMethod.Forecasting, "Контроль с прогназированием"}*/
        };
        private static readonly Dictionary<(LanguageMode, SensorControlAgent), string> ControlAgentsText = new Dictionary<(LanguageMode, SensorControlAgent), string>(){

            { (LanguageMode.Russian, SensorControlAgent.SignalingDevice), "Сигнализатор"},
            { (LanguageMode.Ukrainian, SensorControlAgent.SignalingDevice), "Сигналізатор"},

            { (LanguageMode.Russian, SensorControlAgent.WeakDampedDevice), "Слабодемпфирующее устройство" },
            { (LanguageMode.Ukrainian, SensorControlAgent.WeakDampedDevice), "Слабкодемпфуючий пристрій" },

            { (LanguageMode.Russian, SensorControlAgent.HighlyDampedDevice), "Сильнодемпфирующее устройство"},
            { (LanguageMode.Ukrainian, SensorControlAgent.HighlyDampedDevice), "Сильнодемпфуючий пристрій"},

            { (LanguageMode.Russian, SensorControlAgent.DigitalThreeDigitIndicator), "Цифровой трехразрядный индикатор" },
            { (LanguageMode.Ukrainian, SensorControlAgent.DigitalThreeDigitIndicator), "Цифровий трьохрозрядний індикатор" }


        };
        private static readonly Dictionary<(LanguageMode, InformationRepresentationMethod), string> InformationRepresentationMethodsText = new Dictionary<(LanguageMode, InformationRepresentationMethod), string>() {

            { (LanguageMode.Russian, InformationRepresentationMethod.MnemonicDiagram), "Метод мнемосхем"},
            { (LanguageMode.Ukrainian, InformationRepresentationMethod.MnemonicDiagram), "Метод мнемосхем"},

            { (LanguageMode.Russian, InformationRepresentationMethod.BlockDiagramMethod), "Метод структурных схем"},
            { (LanguageMode.Ukrainian, InformationRepresentationMethod.BlockDiagramMethod), "Метод структурних схем"},

            { (LanguageMode.Russian, InformationRepresentationMethod.GroupMethod), "Группирование по ГВСС на панели"},
            { (LanguageMode.Ukrainian, InformationRepresentationMethod.GroupMethod), "Групування по ГВСС на панелі"}

            //{ InformationRepresentationMethod.GeneralizedMethod, "Метод обобщенного образа"},
            //{ InformationRepresentationMethod.UnorganizedMethod, "Неорганизованное размещение"},
            //{ InformationRepresentationMethod.AutoMethod, "Автоматическое предъявление"},
        };
        private static readonly Dictionary<(LanguageMode, SensorRequestBodies), string> RequestBodiesText = new Dictionary<(LanguageMode, SensorRequestBodies), string>()
        {
            { (LanguageMode.Russian, SensorRequestBodies.ToggleSwitch), "Тумблер"},
            { (LanguageMode.Ukrainian, SensorRequestBodies.ToggleSwitch), "Тумблер"},

            { (LanguageMode.Russian, SensorRequestBodies.Button), "Кнопка"},
            { (LanguageMode.Ukrainian, SensorRequestBodies.Button), "Кнопка"},

            { (LanguageMode.Russian, SensorRequestBodies.Dialer), "Номеронабиратель"},
            { (LanguageMode.Ukrainian, SensorRequestBodies.Dialer), "Номеронабирач"},

            { (LanguageMode.Russian, SensorRequestBodies.KeyboardDevice), "Клавишное устройство"},
            { (LanguageMode.Ukrainian, SensorRequestBodies.KeyboardDevice), "Клавішний пристрій"},
            
            { (LanguageMode.Russian, SensorRequestBodies.None), ""},
            { (LanguageMode.Ukrainian, SensorRequestBodies.None), ""}
        };
        private static readonly Dictionary<(LanguageMode, SensorRequestPlacementMethod), string> RequestPlacementMethodText = new Dictionary<(LanguageMode, SensorRequestPlacementMethod), string>() {

            { (LanguageMode.Russian, SensorRequestPlacementMethod.BlockDiagramMethod), "Метод структурных схем"},
            { (LanguageMode.Ukrainian, SensorRequestPlacementMethod.BlockDiagramMethod), "Метод структурних схем"},

            { (LanguageMode.Russian, SensorRequestPlacementMethod.GroupMethod), "В агрегатной группе"},
            { (LanguageMode.Ukrainian, SensorRequestPlacementMethod.GroupMethod), "В агрегатній групі"},

            { (LanguageMode.Russian, SensorRequestPlacementMethod.MnemonicDiagram), "Размещение на мнемосхеме"},
            { (LanguageMode.Ukrainian, SensorRequestPlacementMethod.MnemonicDiagram), "Розміщення на мнемосхемі"},

            { (LanguageMode.Russian, SensorRequestPlacementMethod.Combination), "Метод совмещения с приборами"},
            { (LanguageMode.Ukrainian, SensorRequestPlacementMethod.Combination), "Метод суміщення з приладами"},

            { (LanguageMode.Russian, SensorRequestPlacementMethod.UnorganizedMethod), "Неорганизованное размещение"},
            { (LanguageMode.Ukrainian, SensorRequestPlacementMethod.UnorganizedMethod), "Неорганізоване розміщення"},

            { (LanguageMode.Russian, SensorRequestPlacementMethod.Separately), "Отдельно"},
            { (LanguageMode.Ukrainian, SensorRequestPlacementMethod.Separately), "Окремо"},

            { (LanguageMode.Russian, SensorRequestPlacementMethod.None), ""},
            { (LanguageMode.Ukrainian, SensorRequestPlacementMethod.None), ""}
        };
        private static readonly Dictionary<(LanguageMode, Words), string> WordsValues = new Dictionary<(LanguageMode, Words), string>() {
            
            { (LanguageMode.Russian, Words.All), "Все"},
            { (LanguageMode.Ukrainian, Words.All), "Всі"},

            { (LanguageMode.Russian, Words.EventID), "события"},
            { (LanguageMode.Ukrainian, Words.EventID), "події"},

            { (LanguageMode.Russian, Words.AddViolation), "Добавить НАРУШЕНИЕ"},
            { (LanguageMode.Ukrainian, Words.AddViolation), "Додати ПОРУШЕННЯ"},

            { (LanguageMode.Russian, Words.AddAuxiliaryParamToVertex), "Добавить ПРИЗНАК к ВЕРШИНЕ"},
            { (LanguageMode.Ukrainian, Words.AddAuxiliaryParamToVertex), "Додати ОЗНАКУ до ВЕРШИНИ"},

            { (LanguageMode.Russian, Words.EventTime), "Время обнаружения события"},
            { (LanguageMode.Ukrainian, Words.EventTime), "Час виявлення події"}
        };

        public static LanguageMode LanguageMode {
            get { return lanMode; }
            set { lanMode = value; }
        }
        static DefaultSettings() {
            lanMode = LanguageMode.Ukrainian;
        }

        public static string getErrorMessageText(ErrorMessages errorMessage) {
            return ErrorMessagesText[(LanguageMode, errorMessage)];
        }
        public static string getMessageBoxText(MessageBoxMessages message)
        {
            return MessageBoxText[(LanguageMode, message)];
        }
        public static string getContextMenuHeaderText(ContextMenuHeaders contextMenuHeader) {
            return ContextMenuHeadersText[(LanguageMode, contextMenuHeader)];
        }
        public static string getWindowTitleText(WindowTitles windowTitle) {
            return WindowsTitlesText[(LanguageMode, windowTitle)];
        }

        public static string getControlMethodText(SensorControlMethod controlMethod) {
            return ControlMethodsText[(LanguageMode, controlMethod)];
        }
        public static string getControlAgentText(SensorControlAgent controlAgent) {
            return ControlAgentsText[(LanguageMode, controlAgent)];
        }
        public static string getInformationRepresentationMethodText(InformationRepresentationMethod method) {
            return InformationRepresentationMethodsText[(LanguageMode, method)];
        }
        public static string getRequestBodyText(SensorRequestBodies requestBody) {
            return RequestBodiesText[(LanguageMode, requestBody)];
        }
        public static string getRequestPlacementMethodText(SensorRequestPlacementMethod requestPlacementMethod) {
            return RequestPlacementMethodText[(LanguageMode, requestPlacementMethod)];
        }

        public static string getButtonText(Buttons button) {
            return ButtonsText[(LanguageMode, button)];
        }
        public static string getLabelText(Labels label) {
            return LabelsText[(LanguageMode, label)];
        }
        public static string getToolTipText(ToolTips toolTip) {
            return ToolTipsText[(LanguageMode, toolTip)];
        }
        public static string getWordValue(Words word) {
            return WordsValues[(LanguageMode, word)];
        }
    }
    
}
