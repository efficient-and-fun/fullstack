import { useEffect, useState } from "react";
import List from "../components/Friends/List/List";

const FriendsPage = () => {
  const [friends, setFriends] = useState<string[]>([]);
  const [users, setUsers] = useState<string[]>([]);
  const token = localStorage.getItem("authToken");
  const url = "/api/users";
  const friendsURL = url + "/friends";

  useEffect(() => {
    const fetchFriends = async () => {
      const response = await fetch(friendsURL, {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });
      const friendsData = await response.json();
      const friendNames = friendsData.map((friend) => {
        return friend.userName;
      });
      setFriends(friendNames);
    };
    fetchFriends();
  }, []);

  useEffect(() => {
    const fetchUsers = async () => {
      const ownUserId = localStorage.getItem("userId");
      const response = await fetch(url, {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });
      const usersData = await response.json();
      console.log(friends);
      const userNames = usersData
        .filter((user) => !friends.includes(user.userName) && user.userId != ownUserId)
        .map((user) => user.userName);

      setUsers(userNames);
    };
    fetchUsers();
  }, [friends]);

  const handleAddClick = async (user: string) => {
    const friendAddURL = friendsURL + `?friendName=${user}`;
    const response = await fetch(friendAddURL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });
    if (response.ok) {
      setUsers((prevUsers) => prevUsers.filter((u) => u !== user));
      setFriends((prevFriends) => [...prevFriends, user]);
    }
  };

  const handleDeleteClick = async (user: string) => {
    const friendAddURL = friendsURL + `?friendName=${user}`;
    const response = await fetch(friendAddURL, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });
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
      />

      <List
        text="All Users"
        items={users}
        handleNoItemsText="No users yet"
        type="add"
        onButtonClick={handleAddClick}
      />
    </div>
  );
};

export default FriendsPage;
