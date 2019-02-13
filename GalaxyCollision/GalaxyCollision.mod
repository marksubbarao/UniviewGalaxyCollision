filepath +:./modules/GalaxyCollision
coord
{
	name		GalaxyCollision
	parent		Extragalactic
	unit		1e+16
	entrydist	0.1
     
    position
    {
        static 0 0 0.5
    }
}

object GalaxyCollision sgOrbitalObject
{
    coord GalaxyCollision    
    
    cameraradius 1
    targetradius 1.0			
    guiName /Galaxy Collision
    
    geometry SG_USES GalaxyCollisionMesh.usesconf
    scalefactor 1.0
    
    

    off

}
