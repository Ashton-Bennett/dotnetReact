const isTokenExpired = (token: string): boolean => {
  const payload = JSON.parse(atob(token.split(".")[1]));
  const now = Math.floor(Date.now() / 1000);
  return payload.exp <= now;
};

export default isTokenExpired;
