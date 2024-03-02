import NextAuth, { NextAuthOptions } from "next-auth";
import DuendeIDS6Provider from "next-auth/providers/duende-identity-server6";

// Add required properties to the session token from the login response.
// See https://next-auth.js.org/getting-started/typescript#module-augmentation

export const authOptions: NextAuthOptions = {
  session: {
    strategy: "jwt",
  },
  providers: [
    DuendeIDS6Provider({
      id: "id-server",
      clientId: "nextApp",
      clientSecret: "secret",
      issuer: "http://localhost:5000",
      authorization: {
        params: { scope: "openid profile auctionApp" },
      },
      idToken: true,
    }),
  ],
  callbacks: {
    async jwt({ token, profile, account, user }) {
      // console.log({ token, profile, account, user });
      if (profile) {
        token.username = profile.username;
      }
      if (account) {
        token.access_token = account.access_token;
      }
      return token;
    },
    async session({ session, token }) {
      if (token) {
        session.user.username = token.username;
      }
      return session;
    },
  },
};

const handler = NextAuth(authOptions);

export { handler as GET, handler as POST };
