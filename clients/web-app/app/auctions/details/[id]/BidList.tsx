"use client";

import Heading from "@/app/components/Heading";
import { Auction } from "@/types";
import { User } from "next-auth";
import React, { useEffect, useState } from "react";
import { toast } from "react-hot-toast";
import EmptyFilter from "@/app/components/EmptyFilter";

type Props = {
  user: User | null;
  auction: Auction;
};

export default function BidList({ user, auction }: Props) {
  const openForBids = new Date(auction.auctionEnd) > new Date();

  return (
    <div className="rounded-lg shadow-md">
      <div className="py-2 px-4 bg-white">
        <div className="sticky top-0 bg-white p-2">
          <Heading title={`Current high bid is $...`} />
        </div>
      </div>

      <div className="overflow-auto h-[400px] flex flex-col-reverse px-2">
        <EmptyFilter
          title="No bids for this item"
          subtitle="Please feel free to make a bid"
        />
      </div>

      <div className="px-2 pb-2 text-gray-500">
        {!openForBids ? (
          <div className="flex items-center justify-center p-2 text-lg font-semibold">
            This auction has finished
          </div>
        ) : !user ? (
          <div className="flex items-center justify-center p-2 text-lg font-semibold">
            Please login to make a bid
          </div>
        ) : user && user.username === auction.seller ? (
          <div className="flex items-center justify-center p-2 text-lg font-semibold">
            You cannot bid on your own auction
          </div>
        ) : (
          <div className="flex items-center justify-center p-2 text-lg font-semibold">
            A bid form
          </div>
        )}
      </div>
    </div>
  );
}
