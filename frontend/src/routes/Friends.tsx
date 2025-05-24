import { useEffect, useState } from "react";
import { friendsApiCall } from "../api/meetUpApi";
import List from "../components/Friends/List/List";

const FriendsPage = () => {
  const [friends, setFriends] = useState<string[]>([]);
  const [users, setUsers] = useState<string[]>([]);
  const token = localStorage.getItem("authToken");
  const friendsEndpoint = "/friends";

  useEffect(() => {
    const method = "GET";
    const fetchFriends = async () => {
      
      const response = await friendsApiCall(friendsEndpoint, method);
      const friends = await response.json();
      const friendNames = friends.map((friend) => {
        return friend.userName;
      });
      setFriends(friendNames);
    };
    fetchFriends();
  }, []);

  useEffect(() =>{
    const method = "GET";
    const fetchUsers = async () => {
      const ownUserId = localStorage.getItem("userId");
      const response = await friendsApiCall("", method);
      const users = await response.json();
      const userNames = users
        .filter((user) => !friends.includes(user.userName) && user.userId != ownUserId)
        .map((user) => user.userName);

      setUsers(userNames);
    };
    fetchUsers();
  }, [friends]);

  const handleAddClick = async (user: string) => {
    const method = "POST";
    const url = friendsEndpoint + `?friendName=${user}`;
    const response = await friendsApiCall(url, method);
    if (response.ok) {
      setUsers((prevUsers) => prevUsers.filter((u) => u !== user));
      setFriends((prevFriends) => [...prevFriends, user]);
    }
  };

  const handleDeleteClick = async (user: string) => {
    const method = "DELETE";
    const friendAddURL = friendsEndpoint + `?friendName=${user}`;
    const response = await friendsApiCall(friendAddURL, method);
    if (response.ok) {
      setFriends((prevFriends) => prevFriends.filter((f) => f !== user));
      setUsers((prevUsers) => [...prevUsers, user]);
    }
  };

  return (
    <div>
      <List
        text="My Friends"
        items={friends}
        handleNoItemsText="Click + to add frineds!"
        type="delete"
        onButtonClick={handleDeleteClick}
        classExtension="cy-friends-friends"
      />

      <List
        text="All Users"
        items={users}
        handleNoItemsText="No users yet"
        type="add"
        onButtonClick={handleAddClick}
        classExtension="cy-friends-users"
      />
    </div>
  );
};

export default FriendsPage;
