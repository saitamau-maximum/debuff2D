# Enemy AI Specification


## ChaserEnemy

### 概要
ChaserEnemy は、プレイヤーを追跡して接近する敵です。プレイヤーを見つけたら、横方向に移動して近づきます。

### 主な動作
- プレイヤーが存在する場合、プレイヤーへ向かって移動する
- プレイヤーとの距離が近づくと停止する
- 段差がある場合、ジャンプで乗り越える
- 壁にぶつかった場合は反対方向へ押し戻されるように移動する

### 設定項目
- Player Target: 追跡対象の Transform
- Move Speed: 移動速度
- Stop Distance: 追跡を止める距離
- Environment Mask: 地形・壁判定に使うレイヤー (**Ground推奨**)
- Ground Check Distance: 接地判定の長さ
- Step Check Distance: 段差判定の距離
- Step Check Height: 段差判定の高さ
- Jump Velocity: ジャンプの上向き速度
- Jump Cooldown: ジャンプのクールダウン時間
- Wall Check Distance: 壁判定の距離
- Wall Push Back Speed: 壁にぶつかったときの反発速度


## SimpleEnemy

### 概要
SimpleEnemy は、プレイヤーを追跡せずに一定方向へ移動する敵です。初期方向を設定しておくことで、左右どちらかへ進むように動作します。

### 主な動作
- 初期方向に向かって移動する
- 追跡しない
- 段差がある場合、ジャンプで乗り越えることができる
- 乗り越えない設定にすると、壁にぶつかったら方向転換する
- 崖に落ちる場合は、落下を許可するかどうかを設定できる

### 設定項目
- Initial Direction: 初期移動方向（Left / Right）
- Move Speed: 移動速度
- Environment Mask: 地形・壁判定に使うレイヤー (**Ground推奨**)
- Ground Check Distance: 接地判定の長さ
**段差を乗り越える**
- Can Jump Over Steps: 段差をジャンプで乗り越えるかどうか
- Step Check Distance: 段差判定の距離
- Step Check Height: 段差判定の高さ
- Jump Velocity: ジャンプの上向き速度
- Jump Cooldown: ジャンプのクールダウン時間
**崖を落ちる**
- Can Drop Down: 崖に落ちることを許可するかどうか
- Drop Check Distance: 崖判定の距離
- Drop Check Depth: 崖判定の深さ
- Wall Check Distance: 壁判定の距離

