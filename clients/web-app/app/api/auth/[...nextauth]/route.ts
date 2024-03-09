import NextAuth from "next-auth";
import { authOptions } from "./authOption";

// Add required properties to the session token from the login response.
// See https://next-auth.js.org/getting-started/typescript#module-augmentation

const handler = NextAuth(authOptions);

export { handler as GET, handler as POST };
