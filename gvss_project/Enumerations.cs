using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gvss_project
{
    public enum LanguageMode { Russian, Ukrainian};
    public enum Labels { LanguageMode, RegisterAuxiliaryParam, Letter, Index, 
        ControlMethod, ControlAgentAuto, ControlAgentRequest, RequestBody, RequestPlacementMethod, 
        Description, Managment, AuxiliaryParamAndViolation,
        GVSSSettings, NumOfTechLines, OperatorLevel, 
        InformationRepresentationMethod, Event, ChooseEvent,
        InformationQuantity, OriginalTime, RequestTime, IrrelevantInformationTime, 
        AgentTimeInfluence, FindControlAgentTime, TotalTime, AddViolation, 
        Violation, ChooseAction, FailureRate, AuxiliaryParam, AddAuxiliaryParamToVertex};
    public enum Buttons { Register, Edit, Delete, GetTime, Add, Change};
    public enum ToolTips { ViolationsAreUnique, ViolationsCanBeParams, CreateNewGVSS, LoadGVSS, SaveGVSS, SavePhoto};
    public enum ErrorMessages { WhileAddRepetative, WhileGVSSFileCorrupted, WhileASVFileCorrupted, TheNameExistsAlready, NotAllFieldsAreFilled};
    public enum MessageBoxMessages { DoYouWantToSaveGVSSBeforeQuit, DeleteVertex, SaveGVSSBeforeLoad, ChangeMode, FullDeleteParam};
    public enum ContextMenuHeaders { DeleteVertex, Edit};
    public enum WindowTitles { ErrorMB, WarningMB, Edit, Add, EventDetectionTime, MathLABResults};
    public enum Words { All, EventID, AddViolation, AddAuxiliaryParamToVertex, EventTime};
    //mat lab ne zabitb


    public enum OperatorLevel { Second = 0, Third, Forth, Fifth/*, Sixth*/ };
    public enum SensorControlMethod { Auto = 0, Request, /*Forecasting,*/ None };
    public enum SensorControlAgent { WeakDampedDevice = 0, HighlyDampedDevice, DigitalThreeDigitIndicator, SignalingDevice, None }
    public enum SensorArrowSign { None = 0, Left, Up, Right, Down };
    public enum SensorRequestPlacementMethod { UnorganizedMethod = 0, MnemonicDiagram, Combination, BlockDiagramMethod, Separately, GroupMethod, None };
    public enum SensorRequestBodies { ToggleSwitch = 0, Button, Dialer, KeyboardDevice, None }
    public enum InformationRepresentationMethod { /*UnorganizedMethod = 0,*/ MnemonicDiagram, GroupMethod, /*AutoMethod,*/ BlockDiagramMethod, /*GeneralizedMethod,*/ None }
}
