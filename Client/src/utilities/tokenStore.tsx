const tokenStore = (() => {
  let accessToken: string | null = null;

  const set = (newToken: string | null) => {
    accessToken = newToken;
  };

  const get = () => accessToken;

  return { set, get };
})();

export default tokenStore;
