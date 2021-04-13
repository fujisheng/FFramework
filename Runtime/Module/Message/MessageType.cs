using UnityEngine;
using System.Collections;

namespace Framework.Module.Message
{
    public class MessageType
    {
        //系统消息
        public const string OpenView = "OpenView";
        public const string CloseView = "CloseView";
        public const string ChangedLanguageType = "ChangedLanguageType";

        //通用 log 提示
        public const string ShowTips = "ShowTips";
        public const string DebugLog = "DebugLog";

        //buff
        public const string AddBuff = "AddBuff";
        public const string RemoveBuff = "RemoveBuff";

        //SDK 广告
        public const string ShowBanner = "ShowBanner";
        public const string HideBanner = "HideBanner";
        public const string ShowInters = "ShowInters";
        public const string ShowReward = "ShowReward";
        public const string RemoveAd = "RemoveAd";

        //SDK 成就 排行榜
        public const string ShowLeaderboard = "ShowLeaderboard";
        public const string ShowAchievement = "ShowAchievement";
        public const string UnlockAchievement = "UnlockAchievement";


        /*-------------------------------------------以下为自定义消息---------------------------------*/

        public const string PointerDown = "PointerDown";
        public const string PointerUp = "PointerUp";
        public const string Turn = "Turn";
        public const string SharpTurn = "SharpTurn";
        public const string StartLevel = "StartLevel";
        public const string StartNextLevel = "StartNextLevel";
        public const string InitMap = "InitMap";
        public const string SetPlayer = "SetPlayer";
        public const string LoadGame = "LoadGame";
        public const string StartGame = "StartGame";
        public const string EndGame = "EndGame";
        public const string MapLoadComplete = "MapLoadComplete";
        public const string PlayerLoadComplete = "PlayerLoadComplete";
        public const string SetCameraTarget = "SetCameraTarget";
        public const string RestartLevel = "RestartLevel";
        public const string CarCollision = "CarCollision";
        public const string PlayerRevive = "PlayerRevive";
        public const string RemoveAllBuff = "RemoveAllBuff";
        public const string AutoTurn = "AutoTurn";
        public const string CreatedNewMapItem = "CreatedNewMapItem";

        public const string RotateCamera = "RotateCamera";
        public const string ShakeCamera = "ShakeCamera";
        public const string SelectCar = "SelectCar";
        public const string UpGradeCar = "UpGradeCar";
        public const string UnlockCar = "UnlockCar";
        public const string StartTurn = "StartTurn";
        public const string UpGradeCarComplete = "UpGradeCarComplate";
        public const string DestroyMapItem = "DestroyMapItem";
        public const string BlurForce = "BlurForce";
        public const string AddTreeScore = "AddTreeScore";
        public const string Vibration = "Vibration";
        public const string PickUpProp = "PickUpProp";
        public const string FinishGame = "FinishGame";
        public const string ReceiveReward = "ReceiveReward";
    }
}
