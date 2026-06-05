using UnityEngine;
using UnityEngine.Rendering.Universal;


/// <summary>
/// プレイヤーがフロアの境界に近づくと、両方のフロアのライトが半分ずつ点灯し、離れるほど片側を強くする
/// 手前に表示されてある壁などはカメラのCullingMaskを切り替えることで、プレイヤーがいるフロアのオブジェクトだけを表示するようにする
/// CullingMaskされるのは、"CullingMainFloorObject"レイヤーに設定されたオブジェクトで、これらはメインフロアのオブジェクトに設定しておく
/// 別の部屋にあるオブジェクトはSortingLayerを"SubObject"にしておく
/// 床のSortingLayerは"SubFloor"にしておく
/// </summary>
public class MoveBetweenFloor : MonoBehaviour
{
    [SerializeField] Light2D mainFloorLight;
    [SerializeField] Light2D subFloorLight;
    [SerializeField] Camera mainCamera;

    SpriteRenderer playerSprite;

    void Start()
    {
        mainFloorLight.enabled = true;
        subFloorLight.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // プレイヤーがフロアの境界に入ったとき、両方のライトを点灯させる
            mainFloorLight.enabled = true;
            subFloorLight.enabled = true;
            playerSprite = col.GetComponentInChildren<SpriteRenderer>();
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // プレイヤーがフロアの境界にいるとき、プレイヤーの位置に応じてライトの強さを調整する
            int isMoveDistance = transform.position.x < col.transform.position.x ? 1 : -1;      // プレイヤーが右側にいる場合はメインフロアのライトを点灯
            if (isMoveDistance == 1)    // プレイヤーが右側にいる場合はメインフロアのライトを点灯
            {
                playerSprite.sortingLayerName = "Default";
                // 全て対象にする
                mainCamera.cullingMask = ~0;
            }
            else    // プレイヤーが左側にいる場合はサブフロアのライトを点灯
            {
                playerSprite.sortingLayerName = "SubObject";
                // CullingMaskを除外する
                mainCamera.cullingMask = ~LayerMask.GetMask("CullingMainFloorObject");
            }

            // プレイヤーがフロアの境界に近いほど両方を半分ずつ点灯し、
            // 離れるほど片側を強くする
            float distanceToMainFloor = Mathf.Abs(transform.position.x - col.transform.position.x);
            float maxDistance = 0.3f; // ライトの強さが0になる距離
            float blend = Mathf.Clamp01(distanceToMainFloor / maxDistance);

            if (isMoveDistance == 1)
            {
                // プレイヤーが右側にいる場合はメインフロアのライトを点灯
                mainFloorLight.intensity = Mathf.Lerp(0.5f, 1f, blend);
                subFloorLight.intensity = Mathf.Lerp(0.5f, 0f, blend);
            }
            else
            {
                // プレイヤーが左側にいる場合はサブフロアのライトを点灯
                mainFloorLight.intensity = Mathf.Lerp(0.5f, 0f, blend);
                subFloorLight.intensity = Mathf.Lerp(0.5f, 1f, blend);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // プレイヤーがフロアの境界から離れたとき、両方のライトを片方だけ点灯させる
            if (mainFloorLight.intensity > subFloorLight.intensity)
            {
                mainFloorLight.enabled = true;
                subFloorLight.enabled = false;
                mainFloorLight.intensity = 1f;
                subFloorLight.intensity = 0f;
            }
            else        // プレイヤーが左側にいる場合はサブフロアのライトを点灯
            {
                mainFloorLight.enabled = false;
                subFloorLight.enabled = true;
                mainFloorLight.intensity = 0f;
                subFloorLight.intensity = 1f;
            }
        }
    }
}