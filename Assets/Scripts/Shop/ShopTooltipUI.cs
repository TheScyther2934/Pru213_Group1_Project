using TMPro;
using UnityEngine;

/// <summary>
/// Tooltip chỉ hiển thị mô tả; bám theo 1 Transform trong world.
/// Gắn script này lên GameObject Tooltip (trong Canvas).
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class ShopTooltipUI : MonoBehaviour
{
    [Header("Bind in Inspector")]
    public RectTransform root;      // RectTransform của Tooltip
    public TMP_Text descText;       // Chỉ hiển thị mô tả
    public RectTransform pointer;   // (tuỳ chọn) mũi tên; có thể để null

    [Header("Follow")]
    public Vector3 worldOffset = new Vector3(0f, 1.2f, 0f); // lệch so với anchor trong world
    public Vector2 screenOffset = new Vector2(0f, 40f);     // lệch pixel trên UI (screen space)
    public float pointerDistance = 10f;                     // khoảng cách mũi tên ra khỏi đáy tooltip

    Canvas _canvas;
    Camera _cam;
    CanvasGroup _group;
    Transform _currentTarget;

    void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _group  = GetComponent<CanvasGroup>();
        _group.alpha = 1f;                      // tránh mờ do alpha
        Hide(true);

        // TUỲ CHỌN: nếu bạn chắc chắn đã gán MainCamera, dòng dưới là đủ.
        // _cam = Camera.main;
        // Nếu chưa chắc, dùng GetCam() để luôn có camera hợp lệ.
        GetCam();
    }

    void Update()
    {
        if (root.gameObject.activeSelf && _currentTarget != null)
            FollowWorld(_currentTarget);
    }

    /// <summary>Hiện tooltip ở anchor target với text mô tả.</summary>
    public void ShowAt(Transform target, string description)
    {
        _currentTarget = target;
        if (descText) descText.text = description ?? "";
        root.gameObject.SetActive(true);
        _group.alpha = 1f;

        GetCam();                      // đảm bảo có camera trước khi follow
        FollowWorld(target);
    }

    /// <summary>Ẩn tooltip.</summary>
    public void Hide(bool immediate = false)
    {
        _currentTarget = null;
        if (immediate)
        {
            _group.alpha = 1f;
            root.gameObject.SetActive(false);
            return;
        }
        root.gameObject.SetActive(false);
    }

    /// <summary>Cập nhật vị trí tooltip theo 1 điểm trong world.</summary>
    public void FollowWorld(Transform worldTarget)
    {
        if (_canvas == null || worldTarget == null) return;

        var cam = GetCam();
        Vector3 worldPos = worldTarget.position + worldOffset;
        Vector3 screenPos;

        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Với Overlay vẫn dùng WorldToScreenPoint; nếu thiếu camera, rơi về center screen
            screenPos = cam != null
                ? cam.WorldToScreenPoint(worldPos)
                : new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);

            root.position = screenPos + (Vector3)screenOffset;
        }
        else
        {
            screenPos = cam != null
                ? cam.WorldToScreenPoint(worldPos)
                : new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform, screenPos, cam, out Vector2 local);
            root.anchoredPosition = local + screenOffset;
        }

        // Điều khiển mũi tên (nếu có)
        if (pointer)
        {
            // đặt mũi tên ở mép dưới tooltip
            pointer.anchoredPosition = new Vector2(0f, -pointerDistance);

            // Nếu chỉ muốn mũi tên luôn chỉ xuống, comment phần xoay dưới.
            if (cam != null)
            {
                Vector2 tip = RectTransformUtility.WorldToScreenPoint(cam, worldTarget.position);
                Vector2 center = root.position;
                Vector2 dir = (tip - center).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                pointer.localRotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }

    /// <summary>Đảm bảo luôn lấy được Camera hợp lệ.</summary>
    Camera GetCam()
    {
        if (_cam != null) return _cam;
        _cam = Camera.main;
        if (_cam == null) _cam = FindObjectOfType<Camera>(); // fallback nếu quên gán MainCamera
        return _cam;
    }
}
