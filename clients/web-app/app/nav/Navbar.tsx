import React from "react";
import { AiOutlineCar } from "react-icons/ai";
import Search from "./Search";
import Logo from "./Logo";
import LoginButton from "./LoginButton";
import { getCurrentUser } from "../actions/authAction";
import UserAction from "./UserAction";

export default async function Navbar() {
  const user = await getCurrentUser();

  return (
    <header className="sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-gray-800 shadow-sm">
      <Logo />
      <Search />
      { user ? ( <UserAction user={user}/> ) : ( <LoginButton /> ) }
    </header>
  );
}
