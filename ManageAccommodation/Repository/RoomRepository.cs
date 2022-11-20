﻿using ManageAccommodation.Models;
using ManageAccommodation.Models.DBObjects;

namespace ManageAccommodation.Repository
{
    public class RoomRepository 
    {
        private ApplicationDbContext dbContext;
        public RoomRepository()
        {
            this.dbContext = new ApplicationDbContext();
        }

        public RoomRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private RoomModel MapDbObjectToModel(Room dbroom)
        {
            RoomModel model = new RoomModel();

            if(dbroom != null)
            {
                model.Iddorm = dbroom.Iddorm;
                model.OccupiedNo = dbroom.OccupiedNo;
                model.VacanciesNo = dbroom.VacanciesNo;
                model.Capacity = dbroom.Capacity;
                model.Status = dbroom.Status;
                model.Idroom = dbroom.Idroom;
                model.PricePerSt = dbroom.PricePerSt;
            }
            return model;
        }

        private Room MapModelToDbObject(RoomModel model)
        {
            //var dormRepo = new DormRepository();

            Room room = new Room();

            if(model != null)
            {
                room.Iddorm = model.Iddorm;
                room.OccupiedNo = model.OccupiedNo;
                room.VacanciesNo = model.VacanciesNo;
                room.Capacity = model.Capacity;
                room.Status = model.Status;
                room.Idroom = model.Idroom;
                room.PricePerSt = model.PricePerSt;
            }
            return room;
        }

        public List<RoomModel> GetAllRoomsInfo()
        {
            List<RoomModel> roomList = new List<RoomModel>();

            foreach(Room dbRoom in dbContext.Rooms)
            {
                roomList.Add(MapDbObjectToModel(dbRoom));
            }
            return roomList;
        }

        public List<RoomModel> GetAllFreeRooms()
        {
            List<RoomModel> roomList = new List<RoomModel>();
            foreach(Room dbRoom in dbContext.Rooms)
            {
                if(dbRoom.VacanciesNo != 0)
                {
                    roomList.Add(MapDbObjectToModel(dbRoom));
                }
            }
            return roomList ;
        }

        public RoomModel GetRoomById(Guid id)
        {
            return MapDbObjectToModel(dbContext.Rooms.FirstOrDefault(x => x.Idroom == id));
        }

        public RoomModel GetRoomByDormId(Guid id)
        {
            return MapDbObjectToModel(dbContext.Rooms.FirstOrDefault(x => x.Iddorm == id));
        }



        public void InsertRoom(RoomModel roomModel)
        {
            roomModel.Idroom = Guid.NewGuid();

            dbContext.Rooms.Add(MapModelToDbObject(roomModel));
            dbContext.SaveChanges();
        }

        public void UpdateRoom(RoomModel roomModel)
        {
            Room existingRoom = dbContext.Rooms.FirstOrDefault(x => x.Idroom == roomModel.Idroom);

            if(existingRoom != null)
            {
                existingRoom.Idroom = roomModel.Idroom;
                existingRoom.OccupiedNo = roomModel.OccupiedNo;
                existingRoom.VacanciesNo = roomModel.VacanciesNo;
                existingRoom.Status = roomModel.Status;
                existingRoom.Capacity = roomModel.Capacity;
                existingRoom.PricePerSt = roomModel.PricePerSt;
                existingRoom.Iddorm = roomModel.Iddorm;
                dbContext.SaveChanges();
            }
        }

        public void UpdateRoomOnAddStudent(RoomModel roomModel)
        {
            Room existingRoom = dbContext.Rooms.FirstOrDefault(x => x.Idroom == roomModel.Idroom);
            if(existingRoom != null)
            {
                if (existingRoom.VacanciesNo == 1)
                {
                    existingRoom.OccupiedNo = roomModel.OccupiedNo + 1;
                    existingRoom.VacanciesNo = roomModel.VacanciesNo - 1;
                    existingRoom.Status = "Ocupied";
                }
                else
                {
                    existingRoom.OccupiedNo = roomModel.OccupiedNo + 1;
                    existingRoom.VacanciesNo = roomModel.VacanciesNo - 1;
                }
                dbContext.SaveChanges();
            }
        }

        public void UpdateRoomOnStudentDelete(RoomModel roomModel)
        {
            Room existingRoom = dbContext.Rooms.FirstOrDefault(x => x.Idroom == roomModel.Idroom); 
            if(existingRoom != null)
            {
                existingRoom.OccupiedNo = roomModel.OccupiedNo - 1;
                existingRoom.VacanciesNo = roomModel.VacanciesNo + 1;
                existingRoom.Status = "Vacancy";
                
                dbContext.SaveChanges();
            }
        }

        public void DeleteRoom(RoomModel roomModel)
        {
            Room existingRoom = dbContext.Rooms.FirstOrDefault(x => x.Idroom == roomModel.Idroom);

            if(existingRoom != null)
            {
                RemoveStudentsOnRoomDeleted(existingRoom.Idroom);
                dbContext.Rooms.Remove(existingRoom);
                dbContext.SaveChanges();
            }
        }
        
        public void RemoveStudentsOnRoomDeleted(Guid id)
        {
            var studs = dbContext.Students.Where(x => x.Idroom == id);
            foreach(var item in studs)
            {
                dbContext.Students.Remove(item);
            }
        }
    }
}
