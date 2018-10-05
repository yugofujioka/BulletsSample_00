using UnityEngine;


/// <summary>
/// 花撃ち
/// </summary>
public sealed class GunFlower : MonoBehaviour {
	#region DEFINE
	private const float SHOT_SPEED = 350f;      // 発射速度（pix./sec.）

	private const float BURST_SPAN = DEFINE.FRAME_TIME_60 * 20f;	// 連続発射間隔（sec.）
	private const int BURST_COUNT = 3;		// 連続発射数
	private const float RAPID_SPAN = DEFINE.FRAME_TIME_60;	// 連射間隔（sec.）
	private const int RAPID_COUNT = 12;     // 連射数
	private const float RAPID_ANGLE = 4f;	// 連射旋回角（deg.）
	private const float RAPID_SPEED = -15f;	// 連射加算速度（pix./sec.）
	
	private static readonly Vector3 ROT_AXIS = Vector3.back;
	private static readonly Vector3 DIRECT_FORWARD = Vector3.down;
	#endregion


	#region MEMBER
	[SerializeField, Tooltip("主弾")]
	private Sprite mainBullet = null;

	private bool fire = false;

	private float shotWait = 0f;    // 発射待ち時間（sec.）
	private int rapidCount = 0;     // 連射カウンタ
	private int burstCount = 0;
	#endregion


	#region MAIN FUNCTION
	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize() {
	}

	/// <summary>
	/// 稼動
	/// </summary>
	public void Run(float elapsedTime) {
		if (!this.fire)
			return;

		this.ProcMain(elapsedTime);
	}
	#endregion


	#region PUBLIC FUNCTION
	/// <summary>
	/// 射撃開始
	/// </summary>
	public void PullTrigger() {
		this.fire = true;
		this.shotWait = DEFINE.FRAME_TIME_60;
		this.rapidCount = 0;
		this.burstCount = 0;
	}

	/// <summary>
	/// 射撃停止
	/// </summary>
	public void ReleaseTrigger() {
		this.fire = false;
	}
	#endregion


	#region PRIVATE FUNCTION
	/// <summary>
	/// メイン射撃処理
	/// </summary>
	/// <param name="elapsedTime">経過時間</param>
	private void ProcMain(float elapsedTime) {
		this.shotWait -= elapsedTime;
		if (this.shotWait > DEFINE.FLOAT_MINIMUM)
			return;

		float passedTime = -this.shotWait;
		this.Shot(passedTime);

		// 超過時間分を再帰計算
		if (this.fire && passedTime > DEFINE.FLOAT_MINIMUM)
			this.ProcMain(passedTime);
	}

	/// <summary>
	/// 直進弾
	/// </summary>
	/// <param name="passedTime">超過時間</param>
	private void Shot(float passedTime) {
		float speed = SHOT_SPEED + RAPID_SPEED * this.rapidCount;
		float angle = this.rapidCount * RAPID_ANGLE + (this.burstCount % 2 == 0 ? 0f : 45f);
		BulletLinear bullet;
		Vector3 dir;
		Vector3 point = Camera.main.WorldToScreenPoint(this.transform.localPosition);
		point.x -= Screen.width * 0.5f;
		point.y -= Screen.height * 0.5f;
		point.z = 0f;
		// 縦
		dir = Quaternion.AngleAxis(angle, ROT_AXIS) * DIRECT_FORWARD;
		if (GameManager.bulletManager.AwakeObject(0, point, out bullet)) {
			BulletLinear bl = bullet as BulletLinear;
			bl.Shoot(this.mainBullet, speed, 0f, ref dir, passedTime);
		}
		dir.x = -dir.x;
		if (GameManager.bulletManager.AwakeObject(0, point, out bullet)) {
			BulletLinear bl = bullet as BulletLinear;
			bl.Shoot(this.mainBullet, speed, 0f, ref dir, passedTime);
		}
		dir.y = -dir.y;
		if (GameManager.bulletManager.AwakeObject(0, point, out bullet)) {
			BulletLinear bl = bullet as BulletLinear;
			bl.Shoot(this.mainBullet, speed, 0f, ref dir, passedTime);
		}
		dir.x = -dir.x;
		if (GameManager.bulletManager.AwakeObject(0, point, out bullet)) {
			BulletLinear bl = bullet as BulletLinear;
			bl.Shoot(this.mainBullet, speed, 0f, ref dir, passedTime);
		}
		// 横
		dir = Quaternion.AngleAxis(angle, ROT_AXIS) * Vector3.right;
		if (GameManager.bulletManager.AwakeObject(0, point, out bullet)) {
			BulletLinear bl = bullet as BulletLinear;
			bl.Shoot(this.mainBullet, speed, 0f, ref dir, passedTime);
		}
		dir.x = -dir.x;
		if (GameManager.bulletManager.AwakeObject(0, point, out bullet)) {
			BulletLinear bl = bullet as BulletLinear;
			bl.Shoot(this.mainBullet, speed, 0f, ref dir, passedTime);
		}
		dir.y = -dir.y;
		if (GameManager.bulletManager.AwakeObject(0, point, out bullet)) {
			BulletLinear bl = bullet as BulletLinear;
			bl.Shoot(this.mainBullet, speed, 0f, ref dir, passedTime);
		}
		dir.x = -dir.x;
		if (GameManager.bulletManager.AwakeObject(0, point, out bullet)) {
			BulletLinear bl = bullet as BulletLinear;
			bl.Shoot(this.mainBullet, speed, 0f, ref dir, passedTime);
		}

		//if (this.rapidCount % 3 == 0) {
		//	speed = 200f;
		//	int way = 13;
		//	float wayAngle = 360f / way;
		//	dir = this.shotDirects[0];
		//	if (this.rapidCount % 6 == 0)
		//		dir = Quaternion.AngleAxis(wayAngle * 0.5f, ROT_AXIS) * dir;
		//	GameManager.BulletWay(BULLET_SPRITE.NAIL_P, ref point, ref dir, speed, 0f, way, wayAngle, passedTime);
		//}

		if (++this.rapidCount == RAPID_COUNT) {
			this.rapidCount = 0;
			if (++this.burstCount == BURST_COUNT) {
				this.burstCount = 0;
				this.ReleaseTrigger();
			} else {
				this.shotWait = BURST_SPAN;
			}
		} else {
			this.shotWait = RAPID_SPAN;
		}
	}
	#endregion
}
