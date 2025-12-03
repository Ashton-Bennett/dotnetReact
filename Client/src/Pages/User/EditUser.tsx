import { useState } from "react";
import UpdatePasswordForm from "../../components/UpdatePasswordForm";
import EditUserForm from "../../components/EditUserForm";

const EditUser = () => {
  const [formToShow, setFormToShow] = useState<"updatePassword" | "editUser">(
    "editUser"
  );

  return (
    <div className="column center f-gap-8">
      {formToShow === "editUser" ? (
        <EditUserForm setFormToShow={setFormToShow} />
      ) : (
        <UpdatePasswordForm setFormToShow={setFormToShow} />
      )}
    </div>
  );
};

export default EditUser;
