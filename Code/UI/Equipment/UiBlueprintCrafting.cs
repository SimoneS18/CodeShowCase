using System;
using Shared.Data.PlayerBase;
using Shared.Enums.Assets;
using Shared.Scriptables.Buildings;
using UI.BaseBuilding.Buildings;
using Utils;

namespace UI.Equipment
{
public class UiBlueprintCrafting : BuildingUI
{
    static public Action<ushort, int> OnFirstOpenUpdateDisplay;

    public override void Init(ushort placeableId, BasePlaceableData data, int index, AdditionalIcon icon)
    {
        if (!Assets.GetBuilding(placeableId, out PlaceableSO placeableSO))
            return;

        OnFirstOpenUpdateDisplay?.Invoke(placeableId, index);
    }
}
}