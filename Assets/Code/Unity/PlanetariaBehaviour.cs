using UnityEngine;

public abstract class PlanetariaBehaviour : Component
{
    public abstract void OnBlockEnter(BlockInteractor block_information);
    public abstract void OnBlockExit(BlockInteractor block_information);
    public abstract void OnBlockStay(BlockInteractor block_information);
    
    public abstract void OnZoneEnter(ZoneInteractor zone_information);
    public abstract void OnZoneExit(ZoneInteractor zone_information);
    public abstract void OnZoneStay(ZoneInteractor zone_information);
}

/*
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/