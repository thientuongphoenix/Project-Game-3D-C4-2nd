# TEMPLATE KỸ THUẬT CHO DỰ ÁN UNITY 3D

## 1. Kiến Trúc Hệ Thống

### 1.1. Core Systems

- Sử dụng Singleton pattern cho các Manager classes
- Tất cả Manager classes kế thừa từ `SaiSingleton<T>`
- Mỗi hệ thống chính nên có một Manager riêng:
  - `SoundManager`: Quản lý âm thanh
  - `ItemsDropManager`: Quản lý item drops
  - `InventoryManager`: Quản lý inventory
  - `EffectSpawnerCtrl`: Quản lý effects

### 1.2. Base Classes

- `SaiMonoBehaviour`: Base class cho tất cả các MonoBehaviour
- `SaiSingleton<T>`: Base class cho các Manager
- `DamageSender`: Base class cho damage system
- `DamageReceiver`: Base class cho damage system
- `EffectCtrl`: Base class cho effects
- `Enemy`: Base class cho enemies

## 2. Design Patterns

### 2.1. Singleton Pattern

```csharp
public abstract class SaiSingleton<T> : SaiMonoBehaviour where T : SaiMonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError(typeof(T).Name + ": Instance chưa được khởi tạo");
            }
            return _instance;
        }
    }
}
```

### 2.2. Object Pooling

- Sử dụng cho tất cả các đối tượng thường xuyên spawn/despawn
- Mỗi pool nên có một Spawner riêng
- Tất cả prefabs cần được đăng ký trong PoolPrefabs

### 2.3. Component Pattern

- Mỗi GameObject nên có một Controller chính
- Controller quản lý các component con
- Sử dụng RequireComponent attribute khi cần

## 3. Coding Standards

### 3.1. Naming Conventions

- PascalCase cho:
  - Class names
  - Public properties
  - Public methods
- camelCase cho:
  - Private fields
  - Local variables
- Prefix `_` cho private fields
- Suffix phù hợp:
  - `Ctrl` cho controllers
  - `Manager` cho managers
  - `Data` cho data classes

### 3.2. Method Structure

```csharp
protected virtual void MethodName()
{
    // 1. Validation
    if (condition) return;

    // 2. Main logic
    DoSomething();

    // 3. Cleanup/Reset
    ResetValues();
}
```

### 3.3. Component Loading

```csharp
protected override void LoadComponents()
{
    base.LoadComponents();
    this.LoadComponentA();
    this.LoadComponentB();
}

protected virtual void LoadComponentA()
{
    if (this.componentA != null) return;
    this.componentA = GetComponent<ComponentA>();
    Debug.Log(transform.name + ": LoadComponentA", gameObject);
}
```

## 4. Systems Architecture

### 4.1. Damage System

- Sử dụng DamageSender và DamageReceiver
- Mỗi loại damage có một sender riêng
- Damage calculation được xử lý trong receiver

### 4.2. Effect System

- Effects được quản lý bởi EffectSpawnerCtrl
- Mỗi effect có một controller riêng
- Effects có thể có damage sender

### 4.3. Inventory System

- Sử dụng ItemCode enum cho item types
- ItemProfileSO cho item data
- InventoryManager quản lý inventory logic

### 4.4. Sound System

- SoundManager quản lý tất cả âm thanh
- Phân biệt giữa Music và SFX
- Volume control riêng cho từng loại

## 5. Best Practices

### 5.1. Performance

- Cache component references
- Sử dụng object pooling
- Tránh FindObjectOfType trong Update
- Tối ưu Update và FixedUpdate

### 5.2. Error Handling

- Validate input parameters
- Log errors với context
- Sử dụng try-catch cho operations có thể fail

### 5.3. Debugging

- Sử dụng Debug.Log với context
- Thêm [SerializeField] cho private fields cần inspect
- Sử dụng Debug.DrawRay cho raycasts

## 6. Scene Structure

### 6.1. Hierarchy Organization

- Managers ở root level
- Environment objects trong \_Env
- UI elements trong \_UI
- Effects trong \_Effects

### 6.2. Prefab Organization

- Tách prefabs theo chức năng
- Sử dụng nested prefabs khi cần
- Đặt tên prefab rõ ràng

## 7. Testing

### 7.1. Unit Tests

- Test core systems
- Test damage calculations
- Test inventory operations

### 7.2. Play Mode Tests

- Test gameplay mechanics
- Test UI flows
- Test performance

## 8. Documentation

### 8.1. Code Documentation

- XML comments cho public methods
- README cho mỗi system
- Architecture diagrams

### 8.2. Scene Documentation

- Scene hierarchy documentation
- Prefab usage documentation
- Setup instructions
