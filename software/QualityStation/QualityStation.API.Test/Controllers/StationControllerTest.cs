using QualityStation.API.Controllers;
using QualityStation.API.Models;
using QualityStation.API.Test.Fixtures;
using QualityStation.API.Test.Utils;
using QualityStation.Shared.ModelDto.AirQualityRecordDto;
using QualityStation.Shared.ModelDto.RecordAttributeDto;
using QualityStation.Shared.ModelDto.StationDto;
using QualityStation.Shared.ModelDto.UserDto;
using System.Text;

namespace QualityStation.API.Test.Controllers
{
    public class StationControllerTest : IClassFixture<ControllerFixture>
    {
        private ControllerFixture m_fControllerFixture;
        private AuthenticationModel m_mFirstUserAuthenInfo = new AuthenticationModel
        {
            Username = "Username",
            Password = "password",
        };

        private AuthenticationModel m_mSecondUserAuthenInfo = new AuthenticationModel
        {
            Username = "Username-second",
            Password = "password-second",
        };


        private StationDto m_mFirstStationDto = new StationDto
        {
            StationName = "test-station",
        };

        private StationDto m_mStationWithNonExistedId = new StationDto
            {
                Id = "non-existed-station-id",
            };


        private string m_strStationPosition = "station-position";

        private RecordAttributeDto m_mFirstAttribute = new RecordAttributeDto
        {
            AttributeName = "Temperature",
            DataType = RecordDataType.Int16,
        };
        private short m_Temperature = 23;

        private RecordAttributeDto m_mSecondAttribute = new RecordAttributeDto
        {
            AttributeName = "Humidity",
            DataType = RecordDataType.Float32,
        };
        private float m_Humidity;

        private RecordAttributeDto m_mThirdAttribute = new RecordAttributeDto
        {
            AttributeName = "Counting",
            DataType = RecordDataType.UInt16,
        };
        private ushort m_Counting = 3;

        private RecordAttributeDto m_mFourthAttribute = new RecordAttributeDto
        {
            AttributeName = "fourth-attribute",
            DataType = RecordDataType.Byte,
        };
        private string m_TestData = "";

        public StationControllerTest(ControllerFixture controllerFixture)
        {   
            byte[] m_TestBytes = new byte[32];
            Array.ConstrainedCopy(BitConverter.GetBytes(m_Temperature), 0, m_TestBytes, 0, 2);
            Array.ConstrainedCopy(BitConverter.GetBytes(m_Humidity), 0, m_TestBytes, 2, 4);
            Array.ConstrainedCopy(BitConverter.GetBytes(m_Counting), 0, m_TestBytes, 6, 2);

            m_TestData = string.Join(" ", m_TestBytes.Select(b => b.ToString("X2")));

            m_fControllerFixture = controllerFixture;
        }

        private async Task<StationDto> CreateStationsWithAttribute(StationController controller, 
                                                    StationDto stationDto,
                                                    List<RecordAttributeDto> records)
        {
            var createStation = ResponseExtraction.GetObjectFromOkResponse(
                            (await controller.CreateStation(stationDto))!);

            foreach (var record in records)
            {
                record.StationId = createStation.Id;
                await controller.AddAttribute(record);
            }

            createStation = ResponseExtraction.GetObjectFromOkResponse(
                            (await controller.GetStationById(createStation.Id))!);

            return createStation;
        }

        [Fact]
        public async void GivenAControllerWithUserLoggedIn_WhenQueryAllStations_ThenReturnEmptyList()
        {
            // Arrage 
            var stationController = await m_fControllerFixture.InitStationController(m_mFirstUserAuthenInfo);

            // Act
            var response = await stationController.QueryAllStations();

            // Assert
            var result = ResponseExtraction.GetListObjectFromOkResponse(response!)
                            .Should().BeNullOrEmpty();
        }

        [Fact]
        public async void GivenAControllerWithUserLoggedIn_WhenAddNewStation_ThenThatUserCanAccessThatStation()
        {
            // Arrange 
            var stationController = await m_fControllerFixture.InitStationController(m_mFirstUserAuthenInfo);

            // Act 
            var response = await stationController.CreateStation(m_mFirstStationDto);

            // Assert
            var station = ResponseExtraction.GetObjectFromOkResponse<StationDto>(response!);
            station.StationName.Should().Be(m_mFirstStationDto.StationName);

            var stations = ResponseExtraction.GetListObjectFromOkResponse<StationDto>(
                            (await stationController.QueryAllStations())!);
            stations.Should().HaveCount(1);
            stations[0].StationName.Should().Be(m_mFirstStationDto.StationName);
            stations[0].Id.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void GivenAControllerWithUserLoggedIn_WhenAddNewStationWithExistedName_ThenThatUserCanAccessThatStation()
        {
            // Arrage
            var stationController = await m_fControllerFixture.InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(stationController, 
                                                m_mFirstStationDto, 
                                                new List<RecordAttributeDto>());

            // Act 
            var response = await stationController.CreateStation(m_mFirstStationDto);

            // Assert
            ResponseExtraction.GetErrorMessageFromConflictResponse<StationDto>(response!)
                                .Should().Be(StationControllerConstant.STATION_EXISTED_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenA2UsersAndAStationWhicIsCreatedByTheFirstOne_WhenGetStationOfTheSecondOne_ThenReturnEmpty()
        {
            // Arrange
            var firstUserStationController = 
                    await m_fControllerFixture.InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController, 
                                                m_mFirstStationDto, 
                                                new List<RecordAttributeDto>());

            var secondUserStationController =
                    await m_fControllerFixture.InitStationController(m_mSecondUserAuthenInfo, 
                                                    bResetService: false);

            // Act
            var response = await secondUserStationController.QueryAllStations();

            // Assert
            ResponseExtraction.GetListObjectFromOkResponse<StationDto>(response)
                    .Should().BeNullOrEmpty();            
        }

        [Fact]
        public async void Given2UsersAndAStationByCreatedByTheFirstOne_WhenTheSecondOneAttachById_ThenTheSecondCanAccessThatStation()
        {
            // Arrange
            var firstUserStationController = 
                    await m_fControllerFixture.InitStationController(m_mFirstUserAuthenInfo);

            var createStation = await CreateStationsWithAttribute(firstUserStationController, 
                                                m_mFirstStationDto, 
                                                new List<RecordAttributeDto>());

            var secondUserStationController =
                    await m_fControllerFixture.InitStationController(m_mSecondUserAuthenInfo,
                                                                        bResetService: false);

            // Act 
            var response = await secondUserStationController.AttachStation(new StationDto
            {
                Id = createStation.Id,
            });

            // Assert
            var mAttachedStation = ResponseExtraction.GetObjectFromOkResponse<StationDto>(response!);
            mAttachedStation.Id.Should().Be(createStation.Id);
            mAttachedStation.StationName.Should().Be(createStation.StationName);

            var mSecondUserStations = ResponseExtraction.GetListObjectFromOkResponse<StationDto>(
                                        (await secondUserStationController.QueryAllStations())!);
            mSecondUserStations.Should().HaveCount(1);
            mSecondUserStations[0].StationName.Should().Be(createStation.StationName);
            mSecondUserStations[0].Id.Should().Be(createStation.Id);
        }

        [Fact]
        public async void Given2UsersWithAStationCreatedByTheFirstOne_WhenTheSecondAttachWithNonExistedId_ThenReturnsNotFoundError()
        {
            // Arrange 
            var firstUserStationController = 
                    await m_fControllerFixture.InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                m_mFirstStationDto,
                                                new List<RecordAttributeDto>());

            var secondUserStationController =
                    await m_fControllerFixture.InitStationController(m_mSecondUserAuthenInfo,
                                                                        bResetService: false);

            // Act 
            var response = await secondUserStationController.AttachStation(new StationDto
            {
                Id = m_mStationWithNonExistedId.Id,
            });

            // Assert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                .Should().Be(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAStation_WhenUpdateTheStationData_ThenTheDataIsChanged()
        {
            // Arrage 
            var firstUserStationController = await m_fControllerFixture.InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                m_mFirstStationDto,
                                                new List<RecordAttributeDto>());

            var newStation = new StationDto
            {
                Id = createStation.Id,
                StationPosition = m_strStationPosition,
            };

            // Act 
            var response = await firstUserStationController.UpdateStation(newStation);

            // Assert
            var requestedStation = ResponseExtraction.GetObjectFromOkResponse<StationDto>(response!);
            requestedStation.Id.Should().Be(createStation.Id);   
            requestedStation.StationName.Should().Be(createStation.StationName);
            requestedStation.StationPosition.Should().Be(m_strStationPosition);
        }

        [Fact]
        public async void GivenAStation_WhenUpdateTheStationWithNonExistedStationId_ThenReturnsNotFound()
        {
            // Arrage 
            var firstUserStationController = 
                        await m_fControllerFixture.InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                m_mFirstStationDto,
                                                new List<RecordAttributeDto>());

            // Act 
            var response = await firstUserStationController.UpdateStation(m_mStationWithNonExistedId);

            // Assert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                    .Should().Be(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void Given2UsersWithAStationWhichIsAttachedToBothUsers_WhenDetachWithTheSecondOne_ThenThatUserCannotAccessToThatStation()
        {
            // Arrange 
            var firstUserStationController = 
                    await m_fControllerFixture.InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                m_mFirstStationDto,
                                                new List<RecordAttributeDto>());

            var secondUserStationController =
                    await m_fControllerFixture.InitStationController(m_mSecondUserAuthenInfo,
                                                                        bResetService: false);
            await secondUserStationController.AttachStation(new StationDto
            {
                Id = createStation.Id,
            });

            // Act 
            var response = await secondUserStationController.DetachStation(new StationDto
            {
                Id = createStation.Id,
            });

            // Assert
            ResponseExtraction.GetObjectFromOkResponse<StationDto>(response!);

            var stations = ResponseExtraction.GetListObjectFromOkResponse<StationDto>(
                            (await secondUserStationController.QueryAllStations())!);

            stations.Should().BeNullOrEmpty();
        }

        [Fact]
        public async void Given2UsersWithAStationWhichIsAttachedToBothUsers_WhenTheSecondOneDetachNonExistedStationId_ThenReturnsNotFound()
        {
            // Arrange 
            var firstUserStationController = 
                    await m_fControllerFixture.InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                m_mFirstStationDto,
                                                new List<RecordAttributeDto>());

            var secondUserStationController =
                    await m_fControllerFixture.InitStationController(m_mSecondUserAuthenInfo,
                                                                        bResetService: false);
            await secondUserStationController.AttachStation(new StationDto
            {
                Id = createStation.Id,
            });

            // Act 
            var response = await secondUserStationController.DetachStation(m_mStationWithNonExistedId);

            // Assert
            ResponseExtraction.GetErrorMessageFromBadRequestResponse(response!)
                            .Should().Be(StationControllerConstant.STATION_IS_NOT_ATTACHED_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAStation_WhenGetStationByExistedId_ThenReturnsThatStation()
        {
            // Arrange
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                m_mFirstStationDto,
                                                new List<RecordAttributeDto>());

            // Act 
            var response = await firstUserStationController.GetStationById(createStation.Id);

            // Assert
            var station = ResponseExtraction.GetObjectFromOkResponse(response!);
            station.StationName.Should().Be(m_mFirstStationDto.StationName);
        }

        [Fact]
        public async void GivenAStation_WhenQueryWithNonExistedId_ThenReturnsNotFound()
        {
            // Arrange
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                 m_mFirstStationDto,
                                                 new List<RecordAttributeDto>());

            // Act
            var response = await firstUserStationController.GetStationById(m_mStationWithNonExistedId.Id);

            // Assert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                    .Should().Be(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);  
        }

        [Fact]
        public async void GivenAStationIsCreated_WhenAddNewAttributes_ThenTheFormatIsSaved()
        {
            // Arrange 
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                  m_mFirstStationDto,
                                                  new List<RecordAttributeDto>());
            m_mFirstAttribute.StationId = createStation.Id;

            // Act 
            var response = await firstUserStationController
                                    .AddAttribute(m_mFirstAttribute);

            // Assert
            var station = ResponseExtraction.GetObjectFromOkResponse(response!);

            station.Attributes.Should().HaveCount(1);
            station.Attributes[0].AttributeName.Should().Be(m_mFirstAttribute.AttributeName);
            station.Attributes[0].AttributeIndex.Should().Be(0);

            station = ResponseExtraction.GetObjectFromOkResponse(
                        (await firstUserStationController.GetStationById(createStation.Id))!);
            station.Attributes.Should().HaveCount(1);
        }

        [Fact]
        public async void GivenAStationWithACreatedAttribute_WhenAddNewAttribute_ThenThatAttributeIndexIs1()
        {
            // Arrange 
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                 m_mFirstStationDto,
                                                 new List<RecordAttributeDto>
                                                 {
                                                     m_mFirstAttribute,
                                                 });
            m_mSecondAttribute.StationId = createStation.Id;

            // Act 
            var response = await firstUserStationController
                                    .AddAttribute(m_mSecondAttribute);

            // Assert
            var mAttributes = ResponseExtraction.GetObjectFromOkResponse(response!).Attributes;

            mAttributes.Should().HaveCount(2);
            var mSecondAttribute = mAttributes
                                .FirstOrDefault(att => att.AttributeName == m_mSecondAttribute.AttributeName);

            mSecondAttribute?.AttributeIndex.Should().Be(1);

            mAttributes = ResponseExtraction.GetObjectFromOkResponse(
                        (await firstUserStationController.GetStationById(createStation.Id))!).Attributes;

            mAttributes.Should().HaveCount(2);
            mSecondAttribute = mAttributes
                                .FirstOrDefault(att => att.AttributeName == m_mSecondAttribute.AttributeName);

            mSecondAttribute?.AttributeIndex.Should().Be(1);
            mSecondAttribute?.DataType.Should().Be(m_mSecondAttribute.DataType);
        }

        [Fact]
        public async void WhenAddAttributeWithNonExistedId_ThenRaiseNotFound()
        {
            // Arrange 
            var stationController = await m_fControllerFixture.InitStationController(m_mFirstUserAuthenInfo);
            m_mFirstAttribute.StationId = m_mStationWithNonExistedId.Id;

            // Act 
            var response = await stationController.AddAttribute(m_mFirstAttribute);

            // Assert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                    .Should().Be(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAStationWith2Attribute_WhenAddNewAttributeWithTheSameName_ThenRaiseConflict()
        {
            // Arrange 
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                 m_mFirstStationDto,
                                                 new List<RecordAttributeDto>
                                                 {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                 });

            // Act 
            var response = await firstUserStationController
                                    .AddAttribute(m_mSecondAttribute);

            // Assert
            ResponseExtraction.GetErrorMessageFromConflictResponse(response!)
                                    .Should().Be(StationControllerConstant.ATTRIBUTE_EXISTED_ERROR_MESSAGE);

            var mAttributes = ResponseExtraction.GetObjectFromOkResponse(
                                (await firstUserStationController.GetStationById(createStation.Id))!).Attributes;

            mAttributes.Should().HaveCount(2);
        }

        [Fact]
        public async void Given4AttributeIsCreated_WhenDeleteTheLastOne_ThenThatOneIsDeleted()
        {
            // Arrange 
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                 m_mFirstStationDto,
                                                 new List<RecordAttributeDto>
                                                 {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                     m_mThirdAttribute,
                                                     m_mFourthAttribute,
                                                 });

            // Act 
            var response = await firstUserStationController
                                    .RemoveAttribute(new RecordAttributeDto
            {
                StationId = createStation.Id,
                AttributeName = m_mFourthAttribute.AttributeName,
            });

            // Assert
            var mAttributes = ResponseExtraction.GetObjectFromOkResponse(response!).Attributes;

            mAttributes.Should().HaveCount(3);

            mAttributes.Any(att => att.AttributeName == m_mFourthAttribute.AttributeName).Should().BeFalse();
        }

        [Fact]
        public async void GivenAStationWith2Attributes_WhenRemoveAttributeOfNonExistedStation_ThenRaiseNotFound()
        {
            // Arrange 
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                 m_mFirstStationDto,
                                                 new List<RecordAttributeDto>
                                                 {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                 });

            // Act 
            var response = await firstUserStationController
                                    .RemoveAttribute(new RecordAttributeDto
            {
                StationId = m_mStationWithNonExistedId.Id,
                AttributeName = m_mSecondAttribute.AttributeName,
            });

            // Assert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                        .Should().Be(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAStationWith2Attributes_WhenRemoveNonExistedAttribute_ThenRaiseNotFound()
        {
            // Arrange 
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                 m_mFirstStationDto,
                                                 new List<RecordAttributeDto>
                                                 {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                 });

            // Act 
            var response = await firstUserStationController
                                    .RemoveAttribute(new RecordAttributeDto
            {
                StationId = createStation.Id,
                AttributeName = m_mThirdAttribute.AttributeName,
            });

            // Assert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                        .Should().Be(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAStationWith4Attribute_WhenDeleteTheNonLastAttribute_ThenAllAttributeAfterThatHasIndexIsIncreased()
        {
            // Arrange 
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                  m_mFirstStationDto,
                                                  new List<RecordAttributeDto>
                                                  {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                     m_mThirdAttribute,
                                                     m_mFourthAttribute,
                                                  });

            // Act 
            var response = await firstUserStationController
                                .RemoveAttribute(new RecordAttributeDto
                                {
                                    StationId = createStation.Id,
                                    AttributeName = m_mSecondAttribute.AttributeName,
                                });

            // Assert
            var mObtainedStationAttributes = ResponseExtraction.GetObjectFromOkResponse(response!).Attributes;
            mObtainedStationAttributes.Should().HaveCount(3);

            var mStationAttributes = ResponseExtraction.GetObjectFromOkResponse(
                                (await firstUserStationController.GetStationById(createStation.Id))!).Attributes;

            mStationAttributes.Should().HaveCount(3);
            var mThirdAttribute = mStationAttributes
                                .FirstOrDefault(att => att.AttributeName == m_mThirdAttribute.AttributeName);

            mThirdAttribute?.AttributeIndex.Should().Be(1);
            var mFourthAttribute = mStationAttributes
                                .FirstOrDefault(att => att.AttributeName == m_mFourthAttribute.AttributeName);

            mFourthAttribute?.AttributeIndex.Should().Be(2);
        }

        [Fact]
        public async void GivenAStationWith4Attributes_WhenSwap2AttributeIndex_ThenTheirIndexesArwSwapped()
        {
            // Arrange
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                  m_mFirstStationDto,
                                                  new List<RecordAttributeDto>
                                                  {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                     m_mThirdAttribute,
                                                     m_mFourthAttribute,
                                                  });

            // Act 
            var response = await firstUserStationController
                               .SwapAttributes(new AttributeSwapRequest
                               {
                                   StationId = createStation.Id,
                                   FirstAttributeName = m_mSecondAttribute.AttributeName,
                                   SecondAttributeName = m_mThirdAttribute.AttributeName,
                               });

            // Assert
            var mObtainedStationAttributes = ResponseExtraction.GetObjectFromOkResponse(response!).Attributes;
            mObtainedStationAttributes.Should().HaveCount(4);

            var mAttributes = ResponseExtraction.GetObjectFromOkResponse(
                                (await firstUserStationController.GetStationById(createStation.Id))!).Attributes;

            var mThirdAttribute = mAttributes
                               .FirstOrDefault(att => att.AttributeName == m_mThirdAttribute.AttributeName);

            mThirdAttribute?.AttributeIndex.Should().Be(1);
            var mSecondAttribute = mAttributes
                                .FirstOrDefault(att => att.AttributeName == m_mSecondAttribute.AttributeName);

            mSecondAttribute?.AttributeIndex.Should().Be(2);
        }

        [Fact]
        public async void GivenAStation_WhenSwapAttributesOfNonExistedStationId_ThenReturnsNotFound()
        {
            // Arrange
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                  m_mFirstStationDto,
                                                  new List<RecordAttributeDto>
                                                  {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                  });

            // Act 
            var response = await firstUserStationController.SwapAttributes(
                                        new AttributeSwapRequest
                                        {
                                            StationId = m_mStationWithNonExistedId.Id,
                                            FirstAttributeName = m_mFirstAttribute.AttributeName,
                                            SecondAttributeName = m_mSecondAttribute.AttributeName,
                                        });

            // Asssert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                    .Should().Be(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAStation_WhenSwapAttributesWithTheFirstOneIsNotExisted_ThenReturnsNotFound()
        {
            // Arrange
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                  m_mFirstStationDto,
                                                  new List<RecordAttributeDto>
                                                  {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                  });

            // Act 
            var response = await firstUserStationController.SwapAttributes(
                                new AttributeSwapRequest
                                {
                                    StationId = createStation.Id,
                                    FirstAttributeName = m_mThirdAttribute.AttributeName,
                                    SecondAttributeName = m_mSecondAttribute.AttributeName,
                                });

            // Asssert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                    .Should().Be(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAStation_WhenSwapAttributesWithTheSecondOneIsNotExisted_ThenReturnsNotFound()
        {
            // Arrange
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                  m_mFirstStationDto,
                                                  new List<RecordAttributeDto>
                                                  {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                  });

            // Act 
            var response = await firstUserStationController.SwapAttributes(
                                        new AttributeSwapRequest
                                        {
                                            StationId = createStation.Id,
                                            FirstAttributeName = m_mFirstAttribute.AttributeName,
                                            SecondAttributeName = m_mFourthAttribute.AttributeName,
                                        });

            // Asssert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                    .Should().Be(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAStationWithAnAttribute_WhenChangeThatAttributeInfo_ThenThatAttributeIsModified()
        {
            // Arrange 
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                  m_mFirstStationDto,
                                                  new List<RecordAttributeDto>
                                                  {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                  });
            var mFirstAttribute = createStation.Attributes
                                    .FirstOrDefault(
                                        att => att.AttributeName == m_mFirstAttribute.AttributeName);
            var mNewAttribute = new RecordAttributeDto
            {
                Id = mFirstAttribute?.Id!,
                StationId = createStation?.Id!,
                AttributeIndex = mFirstAttribute.AttributeIndex,
                AttributeName = m_mThirdAttribute.AttributeName,
                DataType = mFirstAttribute.DataType,
            };

            // Act
            var response = await firstUserStationController.UpdateAttribute(mNewAttribute);

            // Assert
            var mObtainedAttribute = ResponseExtraction.GetObjectFromOkResponse(response!)
                                        .Attributes.FirstOrDefault(att => att.Id == mFirstAttribute?.Id);
            mObtainedAttribute.AttributeName.Should().Be(m_mThirdAttribute.AttributeName);

            var mStoredAttribute = ResponseExtraction.GetObjectFromOkResponse(
                            (await firstUserStationController.GetStationById(createStation.Id))!)
                                        .Attributes.FirstOrDefault(att => att.Id == mFirstAttribute?.Id);
        }

		[Fact]
		public async void GivenAStationWithAnAttribute_WhenChangeThatAttributeInfoWithoutChangeAttributeName_ThenThatAttributeIsModified()
		{
			// Arrange 
			var firstUserStationController = await m_fControllerFixture
												.InitStationController(m_mFirstUserAuthenInfo);
			var createStation = await CreateStationsWithAttribute(firstUserStationController,
												  m_mFirstStationDto,
												  new List<RecordAttributeDto>
												  {
													 m_mFirstAttribute,
													 m_mSecondAttribute,
												  });
			var mFirstAttribute = createStation.Attributes
									.FirstOrDefault(
										att => att.AttributeName == m_mFirstAttribute.AttributeName);
			var mNewAttribute = new RecordAttributeDto
			{
				Id = mFirstAttribute?.Id!,
				StationId = createStation?.Id!,
				AttributeIndex = mFirstAttribute.AttributeIndex,
				AttributeName = m_mFirstAttribute.AttributeName,
				DataType = m_mSecondAttribute.DataType,
			};

			// Act
			var response = await firstUserStationController.UpdateAttribute(mNewAttribute);

			// Assert
			var mObtainedAttribute = ResponseExtraction.GetObjectFromOkResponse(response!)
										.Attributes.FirstOrDefault(att => att.Id == mFirstAttribute?.Id);
			mObtainedAttribute.AttributeName.Should().Be(m_mFirstAttribute.AttributeName);
			mObtainedAttribute.DataType.Should().Be(m_mSecondAttribute.DataType);

			var mStoredAttribute = ResponseExtraction.GetObjectFromOkResponse(
							(await firstUserStationController.GetStationById(createStation.Id))!)
										.Attributes.FirstOrDefault(att => att.Id == mFirstAttribute?.Id);
		}

		[Fact]
		public async void GivenAStationWithAnAttribute_WhenChangeThatAttributeInfoWithExistedName_ThenReturnConflict()
		{
			// Arrange 
			var firstUserStationController = await m_fControllerFixture
												.InitStationController(m_mFirstUserAuthenInfo);
			var createStation = await CreateStationsWithAttribute(firstUserStationController,
												  m_mFirstStationDto,
												  new List<RecordAttributeDto>
												  {
													 m_mFirstAttribute,
													 m_mSecondAttribute,
												  });
			var mFirstAttribute = createStation.Attributes
									.FirstOrDefault(
										att => att.AttributeName == m_mFirstAttribute.AttributeName);
			var mNewAttribute = new RecordAttributeDto
			{
				Id = mFirstAttribute?.Id!,
				StationId = createStation?.Id!,
				AttributeIndex = mFirstAttribute.AttributeIndex,
				AttributeName = m_mSecondAttribute.AttributeName,
				DataType = mFirstAttribute.DataType,
			};

			// Act
			var response = await firstUserStationController.UpdateAttribute(mNewAttribute);

            // Assert
            ResponseExtraction.GetErrorMessageFromConflictResponse(response!)
                    .Should().Be(StationControllerConstant.ATTRIBUTE_EXISTED_ERROR_MESSAGE);
		}

		[Fact]
        public async void GivenAStationWithAnAttribute_WhenChangeThatAttributeOfNonExistedStation_ThenReturnNotFound()
        {
            // Arrange 
            var firstUserStationController = await m_fControllerFixture
                                                .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                  m_mFirstStationDto,
                                                  new List<RecordAttributeDto>
                                                  {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                  });
            var mFirstAttribute = createStation.Attributes
                                    .FirstOrDefault(
                                        att => att.AttributeName == m_mFirstAttribute.AttributeName);
            var mNewAttribute = new RecordAttributeDto
            {
                Id = mFirstAttribute?.Id!,
                StationId = m_mStationWithNonExistedId.Id,
                AttributeIndex = mFirstAttribute.AttributeIndex,
                AttributeName = m_mSecondAttribute.AttributeName,
                DataType = mFirstAttribute.DataType,
            };

            // Act 
            var response = await firstUserStationController.UpdateAttribute(mNewAttribute);

            // Assert 
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                    .Should().Be(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAStationWithAnAttribute_WhenChangeNonExistedAttribute_ThenReturnNotFound()
        {
            var firstUserStationController = await m_fControllerFixture
                                               .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                   m_mFirstStationDto,
                                                   new List<RecordAttributeDto>
                                                   {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                   });
            var mFirstAttribute = createStation.Attributes
                                    .FirstOrDefault(
                                        att => att.AttributeName == m_mFirstAttribute.AttributeName);
            var mNewAttribute = new RecordAttributeDto
            {
                Id = string.Empty,
                StationId = createStation.Id,
                AttributeIndex = mFirstAttribute.AttributeIndex,
                AttributeName = m_mSecondAttribute.AttributeName,
                DataType = mFirstAttribute.DataType,
            };

            // Act 
            var response = await firstUserStationController.UpdateAttribute(mNewAttribute);

            // Assert 
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                    .Should().Be(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAStationWith2Attributes_WhenUpdateTheStationWithNewIndex_ThenNothingHappened()
        {
            // Arrange 
            var firstUserStationController = await m_fControllerFixture
                                               .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                   m_mFirstStationDto,
                                                   new List<RecordAttributeDto>
                                                   {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                   });
            var mFirstAttribute = createStation.Attributes
                                    .FirstOrDefault(
                                        att => att.AttributeName == m_mFirstAttribute.AttributeName);
            var mNewAttribute = new RecordAttributeDto
            {
                Id = mFirstAttribute.Id,
                StationId = createStation.Id,
                AttributeIndex = mFirstAttribute.AttributeIndex + 1,
                AttributeName = m_mThirdAttribute.AttributeName,
                DataType = mFirstAttribute.DataType,
            };

            // Act 
            var response = await firstUserStationController.UpdateAttribute(mNewAttribute);

            // Assert
            var mNewAttributeResult = ResponseExtraction.GetObjectFromOkResponse(response!)
                        .Attributes.FirstOrDefault(att => att.Id == mFirstAttribute.Id);
            mNewAttributeResult.AttributeIndex.Should().Be(mFirstAttribute.AttributeIndex);
        }

        [Fact]
        public async void GivenAStationWith2Attributes_WhenReceiveData_ThenThatDataIsStored()
        {
            // Arrange
            var firstUserStationController = await m_fControllerFixture
                                               .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                   m_mFirstStationDto,
                                                   new List<RecordAttributeDto>
                                                   {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                   });

            await firstUserStationController.AddRecord(new AirQualityRecordDto
            {
                StationId = createStation.Id,
                Data = m_TestData,
            });

            // Act
            var response = await firstUserStationController.GetRecords(createStation.Id);

			// Assert
			var record = ResponseExtraction.GetListObjectFromOkResponse(response!)[0];
			record.Should().Match<Dictionary<string, object>>(r =>
				r.ContainsKey(m_mFirstAttribute.AttributeName) &&
				r[m_mFirstAttribute.AttributeName].Equals(m_Temperature) &&
				r.ContainsKey(m_mSecondAttribute.AttributeName) &&
				r[m_mSecondAttribute.AttributeName].Equals(m_Humidity)
			);
		}

		[Fact]
        public async void GivenAStation_WhenCreateNewRecordWithNonExistedStationId_ThenReturnNotFound()
        {
            // Arrange
            var firstUserStationController = await m_fControllerFixture
                                               .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                   m_mFirstStationDto,
                                                   new List<RecordAttributeDto>
                                                   {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                   });

            // Act
            var response = await firstUserStationController.AddRecord(new AirQualityRecordDto
            {
                StationId = m_mStationWithNonExistedId.Id,
                Data = m_TestData,
            });

            // Assert
            ResponseExtraction.GetErrorMessageFromBadRequestResponse(response!)
                    .Should().Be(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAStationWithACreatedRecord_WhenAskRecordsOfNonExistedStation_ThenReturnNotFound()
        {
            // Arrange 
            var firstUserStationController = await m_fControllerFixture
                                               .InitStationController(m_mFirstUserAuthenInfo);
            var createStation = await CreateStationsWithAttribute(firstUserStationController,
                                                   m_mFirstStationDto,
                                                   new List<RecordAttributeDto>
                                                   {
                                                     m_mFirstAttribute,
                                                     m_mSecondAttribute,
                                                   });

            await firstUserStationController.AddRecord(new AirQualityRecordDto
            {
                StationId = createStation.Id,
                Data = m_TestData,
            });

            // Act 
            var response = await firstUserStationController.GetRecords(m_mStationWithNonExistedId.Id);

            // Assert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                    .Should().Be(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
        }

        [Fact]
        public async void GivenAStationWith3Attributes_WhenGetTheAttributeWithStationIdAndAttributeName_ThenReturnThatAttribute()
        {
			// Arrange
			var firstUserStationController = await m_fControllerFixture
											  .InitStationController(m_mFirstUserAuthenInfo);
			var createStation = await CreateStationsWithAttribute(firstUserStationController,
												   m_mFirstStationDto,
												   new List<RecordAttributeDto>
												   {
													 m_mFirstAttribute,
													 m_mSecondAttribute,
                                                     m_mThirdAttribute,
												   });

            // Act
            var response = await firstUserStationController.GetAttributeByStationIdAndAttributeName(
                                    createStation.Id, m_mFirstAttribute.AttributeName);

            // Assert
            var attribute = ResponseExtraction.GetObjectFromOkResponse(response!);

            attribute.AttributeName.Should().Be(m_mFirstAttribute.AttributeName);
            attribute.AttributeIndex.Should().Be(0);
		}

        [Fact]
        public async void GivenAStationWith3Attributes_WhenGetTheAttributeWithNonExistedStationIdAndAttributeName_ThenReturnsNotFound()
        {
			// Arrange
			var firstUserStationController = await m_fControllerFixture
										  .InitStationController(m_mFirstUserAuthenInfo);
			var createStation = await CreateStationsWithAttribute(firstUserStationController,
												   m_mFirstStationDto,
												   new List<RecordAttributeDto>
												   {
													 m_mFirstAttribute,
													 m_mSecondAttribute,
													 m_mThirdAttribute,
												   });

			// Act 
            var response = await firstUserStationController.GetAttributeByStationIdAndAttributeName(
                                    m_mStationWithNonExistedId.Id, m_mFirstAttribute.AttributeName);

            // Assert
            ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
                    .Should().Be(StationControllerConstant.STATION_DOES_NOT_EXIST_ERROR_MESSAGE);
		}

        [Fact]
        public async void GivenAStationWith3Attributes_WhenGetTheAttributeWithExistedStationIdAndNonExistedAttributeName_ThenReturnsNotFound()
        {
			// Arrange
			var firstUserStationController = await m_fControllerFixture
										  .InitStationController(m_mFirstUserAuthenInfo);
			var createStation = await CreateStationsWithAttribute(firstUserStationController,
												   m_mFirstStationDto,
												   new List<RecordAttributeDto>
												   {
													 m_mFirstAttribute,
													 m_mSecondAttribute,
													 m_mThirdAttribute,
												   });

			// Act 
			var response = await firstUserStationController.GetAttributeByStationIdAndAttributeName(
									createStation.Id, m_mFourthAttribute.AttributeName);

			// Assert
			ResponseExtraction.GetErrorMessageFromNotFoundResponse(response!)
					.Should().Be(StationControllerConstant.ATTRIBUTE_DOES_NOT_EXIST_ERROR_MESSAGE);
		}

	}
}