import useAuth from "../hooks/AuthContext";

const Home = () => {
  const { currentUser } = useAuth();

  return <h1>Welcome Home! {currentUser?.username}</h1>;
};

export default Home;
