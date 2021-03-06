using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;


using DG.Tweening;
namespace DefaultNamespace{
    public class InfoPopup : MonoBehaviour
    {
        [SerializeField]
        private GameObject equpimentPrefab;
        private CanvasGroup _popup;
        private Text _health,_movement,_attack,_defense;
        private Transform _grid;
        private Camera _camera;
        private Unit _unit;

        // Start is called before the first frame update
        void Start()
        {
            _popup = GetComponent<CanvasGroup>();
            _health = transform.Find("Health").GetComponent<Text>();
            _attack = transform.Find("Attack").GetComponent<Text>();
            _defense = transform.Find("Defense").GetComponent<Text>();
            _movement = transform.Find("Movement").GetComponent<Text>();
            _grid = transform.Find("Grid");

            _camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetMouseButtonDown(0))
                CheckWorldSpace();

        }
        private async void CheckWorldSpace(){
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100)){
                
                if(hit.collider.CompareTag("Space")){
                    Debug.Log("Are we here");
                    await DeselectionAnim();    
                    return;
                }

                _unit = hit.collider.GetComponent<Unit>();
                if(_unit == null)
                    return;
                _health.text = "Health:"+_unit.Stats.Health.Value;
                _attack.text = "Attack:"+_unit.Stats.Attack.Value;
                _defense.text = "Defense:"+_unit.Stats.Defence.Value;
                _movement.text = "Movement:"+_unit.Stats.Movement;
                _unit.Equipment.AddedEvent += EquipmentUpdated;
                _unit.Equipment.RemovedEvent += EquipmentUpdated;
                FillEquipmentGrid();
                await SelectionAnim();
                return;
            }
            

        }

        private  void FillEquipmentGrid(){
            foreach(Transform child in _grid)
                Destroy(child.gameObject);
            _unit.Equipment.ToList().ForEach((item)=>{
                if(item==null)
                    return;
                GameObject temp = Instantiate(equpimentPrefab,_grid) as GameObject;
                temp.GetComponent<EquipmentUI>().Setup(item);
                temp.GetComponent<Button>().onClick.AddListener(()=>RemoveEquipment(temp.transform));});
        }
        private async void RemoveEquipment(Transform item){
            item.DOPunchRotation(Vector3.one * 4 ,.5f);
            await item.DOScale(Vector3.zero,.5f).AsyncWaitForCompletion();
            _unit.Equipment.RemoveEquipment(item.GetComponent<EquipmentUI>().equipmentScriptable);
        }
        private void EquipmentUpdated(EquipmentScriptable arg0){
            FillEquipmentGrid();
        }


        //-Anim
        private async Task<bool> SelectionAnim(){
            await _popup.DOFade(1,.6f).AsyncWaitForCompletion();
            return true;
        }
        private async Task<bool> DeselectionAnim(){
            await _popup.DOFade(0,.6f).AsyncWaitForCompletion();
            return true;
        }
    }
}