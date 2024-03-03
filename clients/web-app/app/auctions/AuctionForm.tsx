"use client";

import { Button, CustomFlowbiteTheme, TextInput } from "flowbite-react";
import { FieldValues, useForm } from "react-hook-form";
import Input from "../components/Input";
import { usePathname, useRouter } from "next/navigation";
import DateInput from "../components/DateInput";
import { useEffect } from "react";
import { Auction } from "@/types";
import { createAuction, updateAuction } from "../actions/auctionAction";
import toast from "react-hot-toast";

const customTheme: CustomFlowbiteTheme["button"] = {
  color: {
    gray: "text-gray-500 bg-white border border-gray-300 enabled:hover:bg-gray-100 enabled:hover:text-cyan-700 focus:text-cyan-700 dark:bg-transparent dark:text-gray-400 dark:border-gray-600 dark:enabled:hover:text-white dark:enabled:hover:bg-gray-700",
    blue: "text-blue-100 bg-blue-500 border border-transparent enabled:hover:bg-gray-100 enabled:hover:text-gray-700 dark:bg-blue-500 dark:hover:bg-blue-700",
  },
  size: {
    custom: "text-base px-4 py-1",
    xs: "text-xs px-2 py-1",
    sm: "text-xs px-3 py-1.5",
    md: "text-sm px-4 py-2",
    lg: "text-base px-5 py-2.5",
    xl: "text-base px-6 py-3",
  },
};

type Props = {
  auction?: Auction;
};

export default function AuctionForm({ auction }: Props) {
  const router = useRouter();
  const pathname = usePathname();
  const {
    control,
    handleSubmit,
    setFocus,
    reset,
    formState: { isSubmitting, isValid },
  } = useForm({
    mode: "onTouched",
  });

  useEffect(() => {
    if (auction) {
      const { make, model, color, mileage, year } = auction;
      reset({ make, model, color, mileage, year });
    }
    setFocus("make");
  }, [setFocus, reset, auction]);

  async function onSubmit(data: FieldValues) {
    try {
      let id = "";
      let res;
      if (pathname === "/auctions/create") {
        res = await createAuction(data);
        id = res.id;
      } else {
        if (auction) {
          res = await updateAuction(data, auction.id);
          id = auction.id;
        }
      }
      if (res.error) {
        throw res.error;
      }
      router.push(`/auctions/details/${id}`);
    } catch (error: any) {
      toast.error(error.status + " " + error.message);
    }
  }

  function onCancel() {
    try {
      router.back();
    } catch (error: any) {
      toast.error(error.status + " " + error.message);
    }
  }

  return (
    <form className="flex flex-col mt-3" onSubmit={handleSubmit(onSubmit)}>
      <Input
        label="Make"
        name="make"
        control={control}
        rules={{ required: "Make is required" }}
      />
      <Input
        label="Model"
        name="model"
        control={control}
        rules={{ required: "Model is required" }}
      />
      <Input
        label="Color"
        name="color"
        control={control}
        rules={{ required: "Color is required" }}
      />

      <div className="grid grid-cols-2 gap-3">
        <Input
          label="Year"
          name="year"
          control={control}
          type="number"
          rules={{ required: "Year is required" }}
        />
        <Input
          label="Mileage"
          name="mileage"
          control={control}
          type="number"
          rules={{ required: "Model is required" }}
        />
      </div>

      {pathname === "/auctions/create" && (
        <>
          <Input
            label="Image URL"
            name="imageUrl"
            control={control}
            rules={{ required: "Image URL is required" }}
          />

          <div className="grid grid-cols-2 gap-3">
            <Input
              label="Reserve Price (enter 0 if no reserve)"
              name="reservePrice"
              control={control}
              type="number"
              rules={{ required: "Reserve price is required" }}
            />
            <DateInput
              label="Auction end date/time"
              name="auctionEnd"
              control={control}
              dateFormat="dd MMMM yyyy h:mm a"
              showTimeSelect
              rules={{ required: "Auction end date is required" }}
            />
          </div>
        </>
      )}

      <div className="flex justify-between">
        <Button
          size="md"
          color="warning"
          theme={customTheme}
          onClick={onCancel}
        >
          Cancel
        </Button>
        <Button
          size="md"
          color="blue"
          theme={customTheme}
          isProcessing={isSubmitting}
          type="submit"
        >
          Submit
        </Button>
      </div>
    </form>
  );
}
