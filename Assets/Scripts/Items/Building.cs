using System.Linq;
using DefaultNamespace;
using HexFiled;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Items
{
    [CreateAssetMenu(fileName = "BuildingItem", menuName = "Item/Building")]
    public class Building : Item
    {
        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] private bool isVisiting = false;
        [SerializeField] private bool isPlacableOnAnotherColor = false;
        [SerializeField] private bool isVisible = true;
        [SerializeField] private GameObject aimCanvas;


        public override void Invoke(ItemContainer container)
        {
            if (container.AimInstance == null)
            {
                container.AimInstance = Object.Instantiate(aimCanvas, container.Unit.Instance.transform);
            }

            container.AimInstance.SetActive(false);
        }

        public void Aim(HexDirection direction, ItemContainer container)
        {
            if (container.AimInstance == null)
            {
                container.AimInstance = Object.Instantiate(aimCanvas, container.Unit.Instance.transform);
            }

            container.AimInstance.SetActive(true);


            var cell = HexManager.UnitCurrentCell[container.Unit.Color].cell
                .GetNeighbor(direction);

            if (!isPlacableOnAnotherColor && cell.Color != container.Unit.Color)
            {
                cell = null;
                container.DeAim();
            }

            if (cell == null)
            {
                return;
            }

            container.AimInstance.transform.LookAt(cell.transform);
            container.HexDirection = direction;
        }


        public void PlaceItem(ItemContainer container)
        {
            container.DeAim();
            var cell = HexManager.UnitCurrentCell[container.Unit.Color].cell.GetNeighbor(container.HexDirection);
            if (!isPlacableOnAnotherColor && cell.Color != container.Unit.Color)
            {
                return;
            }

            container.Unit.UseItem(this);
            var obj = Instantiate(buildingPrefab,
                cell.transform.position + buildingPrefab.transform.position, Quaternion.identity);
            obj.GetComponent<ISetUp>().SetUp(container.Unit);
            if (!container.Unit.IsPlayer)
            {
                obj.transform.GetChilds().Where(x => !x.TryGetComponent(typeof(ISetUp), out _))
                    .Select(x => x.gameObject).ToList()
                    .ForEach(x => x.SetActive(false));
                obj.GetComponent<MeshRenderer>().enabled = false;
            }

            if (!isVisiting)
            {
                cell.Building = buildingPrefab;
                cell.BuildingInstance = obj;
            }

            container.OnItemUsed.Invoke();
        }
    }
}