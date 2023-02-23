package CubeSpawnerExtensions;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

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
import com.smartfoxserver.v2.mmo.IMMOItemVariable;
import com.smartfoxserver.v2.mmo.MMOItem;
import com.smartfoxserver.v2.mmo.MMOItemVariable;
import com.smartfoxserver.v2.mmo.MMORoom;
import com.smartfoxserver.v2.mmo.Vec3D;

public class CubeSpawnerRoomExtension extends SFSExtension
{
    private String m_roomGroupId = "cube_spawner";

    private Zone m_zone;
    private MMORoom m_room;
    private ISFSMMOApi m_mmoAPi;

    private SFSArray Cubes = new SFSArray();

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
        addEventHandler(SFSEventType.USER_VARIABLES_UPDATE, new UserVariablesUpdateHandler());
        addRequestHandler("spawn_cube", new SpawnCubeRequestHandler());
        addRequestHandler("spawn_stickerDecal", new SpawnStickerDecalRequestHandler());
    }

    /**
     * Class handling the creation of a static cube.
     *
     * Item will be converted into an MMOItem.
     * This class works the same way as user variables (AOI / Saved in room / Proximity / etc.)
     * but is made for objects in the scene.
     */
    private class SpawnCubeRequestHandler extends BaseClientRequestHandler
    {
        @Override
        public void handleClientRequest(User user, ISFSObject params)
        {
            System.out.print("Sent spawn_cube_from_server event");

            // Creating the MMOItem
            List<IMMOItemVariable> variables = new LinkedList<>();

            variables.add(new MMOItemVariable ("type", "cube"));

            variables.add(new MMOItemVariable("rot", params.getFloat("rot")));
            variables.add(new MMOItemVariable("mat", params.getInt("mat")));
            MMOItem cube = new MMOItem(variables);

            // Adding the MMOItem in the map -> this is enough to make a user see the item in his proximity list
            m_mmoAPi.setMMOItemPosition(cube, new Vec3D(params.getFloat("x"), params.getFloat("y"), params.getFloat("z")), getParentRoom());

            // Sending the item instantly to the users that can see it, to avoid that the client has to wait for the next proximityListUpdate because this can cause some asynchronous behaviour.
            params.putInt("id", cube.getId());
            List<User> usersNearCube = m_room.getProximityList(user);
            usersNearCube.add(user);
            send("spawn_cube_from_server", params, usersNearCube);
        }
    }



    /**
     * Class handling the User Variable Update event occurring in the parent Room.
     *
     * If the User Variables representing user position along x, y or z axis are updated,
     * update the position in MMORoom's proximity manager too to make users appear in each other's Area of Interest.
     *
     * Y axis is not necessary in certain situation, so don't hesitate to minimize the data transfer.
     * (The example I took this code from didn't used the y axis)
     */
    private class UserVariablesUpdateHandler extends BaseServerEventHandler
    {
        @Override
        public void handleServerEvent(ISFSEvent event)
        {
            @SuppressWarnings("unchecked")
            List<UserVariable> variables = (List<UserVariable>) event.getParameter(SFSEventParam.VARIABLES);
            User user = (User) event.getParameter(SFSEventParam.USER);

            // Make a map of the variables list
            Map<String, UserVariable> varMap = new HashMap<String, UserVariable>();

            for (UserVariable var : variables)
            {
                varMap.put(var.getName(), var);
            }

            if (varMap.containsKey("x") || varMap.containsKey("y") || varMap.containsKey("z"))
            {
                // Extract position from User Variables
                Vec3D pos = new Vec3D(
                        varMap.get("x").getDoubleValue().floatValue(),
                        varMap.get("y").getDoubleValue().floatValue(),
                        varMap.get("z").getDoubleValue().floatValue()
                );

                // Set position in proximity system
                m_mmoAPi.setUserPosition(user, pos, getParentRoom());
            }
        }
    }

    private class SpawnStickerDecalRequestHandler extends BaseClientRequestHandler
    {
        @Override
        public void handleClientRequest(User user, ISFSObject params)
        {
            System.out.print("Sent spawn_cube_from_server event");

            // Creating the MMOItem
            List<IMMOItemVariable> variables = new LinkedList<>();

            variables.add(new MMOItemVariable("type", "sticker_decal")); // Specify type of decal


            variables.add(new MMOItemVariable("rotX", params.getFloat("rotX")));
            variables.add(new MMOItemVariable("rotY", params.getFloat("rotY")));
            variables.add(new MMOItemVariable("rotZ", params.getFloat("rotZ")));

            variables.add(new MMOItemVariable("sizeX", params.getFloat("sizeX")));
            variables.add(new MMOItemVariable("sizeY", params.getFloat("sizeY")));
            variables.add(new MMOItemVariable("sizeZ", params.getFloat("sizeZ")));

            variables.add(new MMOItemVariable("stickerID", params.getInt("stickerID")));
            MMOItem stickerDecal = new MMOItem(variables);

            // Adding the MMOItem in the map -> this is enough to make a user see the item in his proximity list
            m_mmoAPi.setMMOItemPosition(stickerDecal, new Vec3D(params.getFloat("x"), params.getFloat("y"), params.getFloat("z")), getParentRoom());

            // Sending the item instantly to the users that can see it, to avoid that the client has to wait for the next proximityListUpdate because this can cause some asynchronous behaviour.
            params.putInt("stickerDecalID", stickerDecal.getId());
            List<User> usersNearCube = m_room.getProximityList(user);
            usersNearCube.add(user);
            send("spawn_stickerDecal_from_server", params, usersNearCube);
        }
    }
}
