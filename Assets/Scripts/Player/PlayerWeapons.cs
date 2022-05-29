using UnityEngine;
using System.Collections;
using Photon.Bolt;

public class PlayerWeapons : Photon.Bolt.EntityBehaviour<IPlayerState>
{
    [SerializeField]
    private Camera _cam = null;
    [SerializeField]
    private Weapon[] _weapons = null;
    private int _weaponIndex = 1;

    [SerializeField]
    private WeaponID _primaryWeapon = WeaponID.None;
    [SerializeField]
    private WeaponID _secondaryWeapon = WeaponID.None;

    private WeaponID _primary = WeaponID.None;
    private WeaponID _secondary = WeaponID.None;

    public int WeaponIndex { get => _weaponIndex; }
    public Camera Cam { get => _cam; }

    [SerializeField]
    private Transform _weaponsTransform = null;
    [SerializeField]
    private GameObject[] _weaponPrefabs = null;

    public void Init()
    {
        Debug.Log("Init in PlayerWeapons.cs is running");
        if (entity.IsOwner)
        {
            Debug.Log("entity.IsOwner returned true");
            for (int i = 0; i < 4; i++)
            {
                state.Weapons[i].CurrentAmmo = -1;
            }
            AddWeaponEvent(_primaryWeapon);
            AddWeaponEvent(_secondaryWeapon);
            //running on server
            Debug.Log("added _primaryWeapon and _secondaryWeapon to AddWeaponEvent");
        }
        StartCoroutine(SetWeapon());
        _weapons[0].Init(this,0);
        //Debug.Log("State of is owner : ");
        //Debug.Log(entity.IsOwner);
        Debug.Log("Init weapons complete");
    }

    IEnumerator SetWeapon()
    {
        while (_weapons[_weaponIndex] == null)
            yield return new WaitForEndOfFrame();
        SetWeapon(_weaponIndex);
        Debug.Log("SetWeapon has run");
    }



    public void ExecuteCommand(bool fire, bool aiming, bool reload, int wheel, int seed, bool drop)
    {
        if(wheel != state.WeaponIndex)
        {
            if (_weapons[wheel] != null)
                if (entity.IsOwner)
                    state.WeaponIndex = wheel;
            Debug.Log(wheel);
        }
        //Debug.Log("ExecuteCommand in PlayerWeapons.cs is running"); confirmed to be running
        if(_weapons[_weaponIndex])
            _weapons[_weaponIndex].ExecuteCommand(fire, aiming, reload, seed);

        DropCurrent(drop);
        
    }

    public void FireEffect(int seed, float precision)
    {
        _weapons[_weaponIndex].FireEffect(seed, precision);
    }

    public void InitAmmo(int i, int current, int total)
    {
        //Debug.Log(i);
        if (_weapons[i] && i != 0)
            _weapons[i].InitAmmo(current, total);
        Debug.Log("ran InitAmmo) in PlayerWeapons.cs");
    }

    public void SetWeapon(int index)
    {
        _weaponIndex = index;

        for (int i = 0; i < _weapons.Length; i++)
            if (_weapons[i] != null)
                _weapons[i].gameObject.SetActive(false);

        _weapons[_weaponIndex].gameObject.SetActive(true);
        Debug.Log("ran SetWeapon in PlayerWeapons.cs");
    }

    public int CalculateIndex(float valueToAdd)
    {
        int i = _weaponIndex;
        int factor = 0;

        if (valueToAdd > 0)
            factor = 1;
        else if (valueToAdd < 0)
            factor = -1;

        i += factor;

        if (i == -1)
            i = _weapons.Length - 1;

        if (i == _weapons.Length)
            i = 0;

        while (_weapons[i] == null)
        {
            i += factor;
            i = i % _weapons.Length;
        }

        return i;
    }

    bool _dropPressed = false;

    public void DropCurrent(bool drop)
    {
        if (drop)
        {
            if (_dropPressed == false)
            {
                _dropPressed = true;
                DropWeapon();

                if (entity.IsOwner)
                    state.WeaponIndex = CalculateIndex(1);
            }
        }
        else
        {
            if (_dropPressed)
            {
                _dropPressed = false;
            }
        }
    }


    public void DropWeapon()
    {
        if (entity.IsOwner)
        {
            if (_weaponIndex != 0)
            {
                WeaponDropToken token = new WeaponDropToken();
                token.currentAmmo = _weapons[_weaponIndex].CurrentAmmo;
                token.totalAmmo = _weapons[_weaponIndex].TotalAmmo;
                token.ID = _weapons[_weaponIndex].WeaponStat.ID;
                token.networkId = entity.NetworkId;

                BoltNetwork.Instantiate(_weapons[_weaponIndex].WeaponStat.drop, token, Cam.transform.position + Cam.transform.forward, Quaternion.LookRotation(Cam.transform.forward));

                if (_weapons[_weaponIndex].WeaponStat.ID < WeaponID.SecondaryEnd)
                    _secondary = WeaponID.None;
                else
                    _primary = WeaponID.None;

                state.Weapons[_weaponIndex].ID = -1;
                Destroy(_weapons[_weaponIndex].gameObject);
                _weapons[_weaponIndex] = null;
                Debug.Log("DropWeapon in PlayerWeapons.cs has run (this should only run if _weaponIndex != 0");
            }
            Debug.Log("DropWeapon in PlayerWeapons.cs has run (this should only run on server side");
        }
        Debug.Log("DropWeapon in PlayerWeapons.cs has run");
    }

    public void RemoveWeapon(int i)
    {
        if (_weapons[i])
            Destroy(_weapons[i].gameObject);

        _weapons[i] = null;
        Debug.Log("ran RemoveWeapon in PlayerWeapons.cs");
    }

    public bool CanAddWeapon(WeaponID toAdd)
    {
        if (toAdd < WeaponID.SecondaryEnd)
        {
            if (_secondary == WeaponID.None)
                return true;
        }
        else
        {
            if (_primary == WeaponID.None)
                return true;
        }
        return false;
        Debug.Log("ran CanAddWeapon in PlayerWeapons.cs");
    }
    
    public void AddWeaponEvent(int i, int ca, int ta)
    {
        if (i < (int)WeaponID.SecondaryEnd)
        {
            state.Weapons[1].ID = i;
            state.Weapons[1].CurrentAmmo = ca;
            state.Weapons[1].TotalAmmo = ta;
        }
        else
        {
            state.Weapons[2].ID = i;
            state.Weapons[2].CurrentAmmo = ca;
            state.Weapons[2].TotalAmmo = ta;
        }
        Debug.Log("ran AddWeaponEvent(int i, int ca, int ta) in PlayerWeapons.cs");
    }

    public void AddWeapon(WeaponID id)
    {
        if (id == WeaponID.None)
            return;

        GameObject prefab = null;
        foreach (GameObject w in _weaponPrefabs)
        {
            if(w.GetComponent<Weapon>().WeaponStat.ID == id)
            {
                prefab = w;
                break;
            }
        }

        prefab = Instantiate(prefab, _weaponsTransform.position, Quaternion.LookRotation(_weaponsTransform.forward), _weaponsTransform);

        if(id < WeaponID.SecondaryEnd)
        {
            _secondary = id;
            _weapons[1] = prefab.GetComponent<Weapon>();
            prefab.GetComponent<Weapon>().Init(this, 1);
        }
        else
        {
            _primary = id;
            _weapons[2] = prefab.GetComponent<Weapon>();
            prefab.GetComponent<Weapon>().Init(this, 2);
        }
        Debug.Log("ran AddWeapon in PlayerWeapons.cs");
    }

    public void AddWeaponEvent(WeaponID id)
    {
        if (id == WeaponID.None)
            return;

        int i = (id < WeaponID.SecondaryEnd) ? 1 : 2;
        state.Weapons[i].ID = (int)id;
        Debug.Log("ran AddWeaponEvent(WeaponID id) in PlayerWeapons.cs");
    }
}
