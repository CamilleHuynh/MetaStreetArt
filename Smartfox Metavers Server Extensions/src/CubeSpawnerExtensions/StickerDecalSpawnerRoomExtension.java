package CubeSpawnerExtensions;

import com.smartfoxserver.v2.SmartFoxServer;
import com.smartfoxserver.v2.api.ISFSMMOApi;
import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.core.SFSEventType;
import com.smartfoxserver.v2.entities.SFSRoomRemoveMode;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.Zone;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSArray;
import com.smartfoxserver.v2.entities.variables.UserVariable;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;
import com.smartfoxserver.v2.extensions.SFSExtension;
import com.smartfoxserver.v2.mmo.*;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

public class StickerDecalSpawnerRoomExtension extends SFSExtension
{
    private String m_roomGroupId = "cube_spawner";

    private Zone m_zone;
    private MMORoom m_room;
    private ISFSMMOApi m_mmoAPi;

    private SFSArray StickerDecal = new SFSArray();

    // Main class is just required for jar compiling with IntelliJ Idea
    public static void main(String[] args) { }

    @Override
    public void init()
    {
        m_zone = getParentZone();
        m_room = (MMORoom) getParentRoom();
        m_mmoAPi = SmartFoxServer.getInstance().getAPIManager().getMMOApi();

        if (m_zone == null || m_room == null)
        {
            trace("CubeSpawnerRoomExtension script must be linked to a room to be used. ParentZone or ParentRoom is currently null.");
            return;
        }

        // Set room group public so the client can subscribe to it
        List<String> publicRoomGroups = m_zone.getPublicGroups();
        if (!publicRoomGroups.contains(m_roomGroupId))
        {
            publicRoomGroups.add(m_roomGroupId);
            m_zone.setPublicGroups(publicRoomGroups);
        }

        // Make the room static (otherwise, room will be automatically deleted if empty of users)
        m_room.setDynamic(false);
        m_room.setAutoRemoveMode(SFSRoomRemoveMode.NEVER_REMOVE);

        // Add event listener
        addRequestHandler("spawn_stickerDecal", new SpawnStickerDecalRequestHandler());
    }

    /**
     * Class handling the creation of a static cube.
     *
     * Item will be converted into an MMOItem.
     * This class works the same way as user variables (AOI / Saved in room / Proximity / etc.)
     * but is made for objects in the scene.
     */
    private class SpawnStickerDecalRequestHandler extends BaseClientRequestHandler
    {
        @Override
        public void handleClientRequest(User user, ISFSObject params)
        {
            System.out.print("Sent spawn_cube_from_server event");

            // Creating the MMOItem
            List<IMMOItemVariable> variables = new LinkedList<>();
            variables.add(new MMOItemVariable("stickerDecalID", params.getInt("id")));

            variables.add(new MMOItemVariable("rotX", params.getFloat("rotX")));
            variables.add(new MMOItemVariable("rotY", params.getFloat("rotY")));
            variables.add(new MMOItemVariable("rotZ", params.getFloat("rotZ")));

            // variables.add(new MMOItemVariable("sizeX", params.getFloat("sizeX")));
            // variables.add(new MMOItemVariable("sizeY", params.getFloat("sizeY")));
            // variables.add(new MMOItemVariable("sizeZ", params.getFloat("sizeZ")));

            variables.add(new MMOItemVariable("stickerID", params.getInt("stickerID")));
            MMOItem stickerDecal = new MMOItem(variables);

            // Adding the MMOItem in the map -> this is enough to make a user see the item in his proximity list
            m_mmoAPi.setMMOItemPosition(stickerDecal, new Vec3D(params.getFloat("x"), params.getFloat("y"), params.getFloat("z")), getParentRoom());

            // Sending the item instantly to the users that can see it, to avoid that the client has to wait for the next proximityListUpdate because this can cause some asynchronous behaviour.
            params.putInt("id", stickerDecal.getId());
            List<User> usersNearCube = m_room.getProximityList(user);
            usersNearCube.add(user);
            send("spawn_stickerDecal_from_server", params, usersNearCube);
        }
    }
}
