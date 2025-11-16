using UnityEngine;

public class BoatFuelManager : MonoBehaviour
{
    public float fuelAmount = 100;

    public float maxFuelAmount = 100;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            fuelAmount += 10;
            if (fuelAmount > maxFuelAmount)
            {
                fuelAmount = maxFuelAmount;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            fuelAmount -= 10;
            if (fuelAmount < 0)
            {
                fuelAmount = 0;
            }
        }
    }
    public void AddFuel(float amount)
    {
        fuelAmount += amount;
    }

    public void DepleteFuel(float amount)
    {
        fuelAmount -= amount;
    }

    public void UpgradeMaxFuel(float upgradeAmount)
    {
        maxFuelAmount += upgradeAmount;
    }
}
