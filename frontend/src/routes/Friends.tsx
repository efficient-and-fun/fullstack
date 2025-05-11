import { useEffect, useState } from 'react';
import List from '../components/Friends/List/List';

const FriendsPage = () => {
  const [friends, setFriends] = useState<string[]>([]);
  const [users, setUsers] = useState<string[]>([]);

  useEffect(() => {
    const fetchFriends = async () => {
      const friendsData = await new Promise<string[]>((resolve) =>
        setTimeout(() => resolve(['Alice', 'Bob', 'Charlie']), 1000)
      );
      setFriends(friendsData);
    };

    const fetchUsers = async () => {
      const usersData = await new Promise<string[]>((resolve) =>
        setTimeout(() => resolve(['Alice', 'Bob', 'Charlie', 'David', 'Eve']), 1000)
      );
      setUsers(usersData);
    };

    fetchFriends();
    fetchUsers();
  }, []);

  const handleAddClick = (user: string) => {
    alert('Add button clicked! ' + user);
  };

  const handleDeleteClick = (user: string) => {
    alert('Delete button clicked! ' + user);
  };

  return (
    <div>
      <List
        text='My Friends'
        items={friends}
        handleNoItemsText="No friends yet"
        type='delete'
        onButtonClick={handleDeleteClick}
      />

      <List
        text='All Users'
        items={users}
        handleNoItemsText="No users yet"
        type='add'
        onButtonClick={handleAddClick}
      />
    </div>
  );
};

export default FriendsPage;