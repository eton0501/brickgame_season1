Brickgame
這是一款基於Unity開發的3D科幻風格打磚塊遊戲。本專案著重於展示**乾淨的程式碼架構**、**資料驅動的關卡生成**，以及**流暢的遊戲打擊感**。

🎮 **[點擊這裡立即在瀏覽器上試玩](https://eton0501.github.io/brickgame_season1/)**

## 📸 遊戲畫面
<img width="964" height="541" alt="螢幕擷取畫面 2026-03-20 190152" src="https://github.com/user-attachments/assets/c0b6c294-c27b-4fe0-ab34-652fdf8c315b" />

## 🛠️ 核心技術

**動態關卡生成：** 透過二維陣列與雙層for迴圈讀取藍圖，動態生成多種陣型與不同 HP 的磚塊。
**狀態機與單例模式：** 透過GameManager.cs區分MENU, PLAY, GAMEOVER 等狀態，嚴格控管遊戲生命週期。
**物理與數學運算：** 實作相對位置反射，球擊中玩家飛船的不同位置會產生不同的物理反彈角度。
**外部套件整合：** 導入套件 **DOTween** 處理 UI 補間動畫，取代生硬的SetActive，提升視覺回饋。

## 📂 程式碼導覽
本專案的完整邏輯皆為獨立撰寫，且每行都有註釋，歡迎檢視以下核心腳本：
* [`GameManager.cs`](https://github.com/eton0501/brickgame_season1/blob/b2771af3d9c4a8eb6471d47d3b15f3e76401fea7/Assets/scripts/GameManager.cs)：負責單例模式、有限狀態機流程控管，以及運用雙層迴圈的動態關卡生成邏輯。
* [`Ball.cs`](https://github.com/eton0501/brickgame_season1/blob/b2771af3d9c4a8eb6471d47d3b15f3e76401fea7/Assets/scripts/Ball.cs)：實作自訂物理碰撞反彈邏輯、防呆防卡死機制，以及根據玩家擋板擊中點計算相對反彈角度。
* [`Brick.cs`](https://github.com/eton0501/brickgame_season1/blob/b2771af3d9c4a8eb6471d47d3b15f3e76401fea7/Assets/scripts/Brick.cs)：實作多段血量系統、利用陣列與亂數判斷的機率性道具掉落，以及擊中瞬間的閃爍特效。
* [`Player.cs`](https://github.com/eton0501/brickgame_season1/blob/b2771af3d9c4a8eb6471d47d3b15f3e76401fea7/Assets/scripts/Player.cs)：實作攝影機座標轉換限制移動範圍，並運用 Coroutine防呆控管道具狀態的持續時間。
* [`PropDown.cs`](https://github.com/eton0501/brickgame_season1/blob/b2771af3d9c4a8eb6471d47d3b15f3e76401fea7/Assets/scripts/PropDown.cs)：運用(`Quaternion`)進行向量旋轉運算，並以列舉 (`Enum`) 乾淨地管理不同的道具效果。
