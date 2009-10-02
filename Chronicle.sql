/*
SQLyog Community Edition- MySQL GUI v8.15 Beta1
MySQL - 5.1.36-community : Database - chronicle
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`chronicle` /*!40100 DEFAULT CHARACTER SET latin1 */;

USE `chronicle`;

/*Table structure for table `account` */

DROP TABLE IF EXISTS `account`;

CREATE TABLE `account` (
  `identifier` int(10) NOT NULL AUTO_INCREMENT,
  `username` varchar(16) NOT NULL,
  `password` varchar(16) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `level` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`identifier`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

/*Table structure for table `player` */

DROP TABLE IF EXISTS `player`;

CREATE TABLE `player` (
  `identifier` int(10) NOT NULL AUTO_INCREMENT,
  `account_identifier` int(10) NOT NULL,
  `name` varchar(12) NOT NULL,
  `gender` tinyint(3) unsigned NOT NULL,
  `skin` tinyint(3) unsigned NOT NULL,
  `eyes_identifier` int(10) NOT NULL,
  `hair_identifier` int(10) NOT NULL,
  `level` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `job` smallint(5) unsigned NOT NULL DEFAULT '0',
  `strength` smallint(5) unsigned NOT NULL DEFAULT '12',
  `dexterity` smallint(5) unsigned NOT NULL DEFAULT '5',
  `intellect` smallint(5) unsigned NOT NULL DEFAULT '4',
  `luck` smallint(5) unsigned NOT NULL DEFAULT '4',
  `health` smallint(5) unsigned NOT NULL DEFAULT '50',
  `max_health` smallint(5) unsigned NOT NULL DEFAULT '50',
  `mana` smallint(5) unsigned NOT NULL DEFAULT '5',
  `max_mana` smallint(5) unsigned NOT NULL DEFAULT '5',
  `ability_points` smallint(5) unsigned NOT NULL DEFAULT '0',
  `skill_points` smallint(5) unsigned NOT NULL DEFAULT '0',
  `experience` int(10) NOT NULL DEFAULT '0',
  `fame` smallint(5) unsigned NOT NULL DEFAULT '0',
  `map_identifier` int(10) NOT NULL DEFAULT '0',
  `map_spawn` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `buddy_slots` tinyint(3) unsigned NOT NULL DEFAULT '20',
  `equipment_slots` tinyint(3) unsigned NOT NULL DEFAULT '24',
  `use_slots` tinyint(3) unsigned NOT NULL DEFAULT '24',
  `setup_slots` tinyint(3) unsigned NOT NULL DEFAULT '24',
  `etc_slots` tinyint(3) unsigned NOT NULL DEFAULT '24',
  `cash_slots` tinyint(3) unsigned NOT NULL DEFAULT '48',
  `mesos` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`identifier`),
  UNIQUE KEY `UNIQUE_NAME` (`name`),
  KEY `INDEX_ACCOUNT_IDENTIFIER` (`account_identifier`),
  CONSTRAINT `FK_PLAYER_ACCOUNT_TO_ACCOUNT` FOREIGN KEY (`account_identifier`) REFERENCES `account` (`identifier`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

/*Table structure for table `player_buddy` */

DROP TABLE IF EXISTS `player_buddy`;

CREATE TABLE `player_buddy` (
  `player_identifier` int(10) NOT NULL,
  `buddy_identifier` int(10) NOT NULL,
  `name` varchar(12) NOT NULL,
  `status` tinyint(3) unsigned NOT NULL,
  KEY `INDEX_PLAYER_IDENTIFIER` (`player_identifier`),
  KEY `INDEX_BUDDY_IDENTIFIER` (`buddy_identifier`),
  CONSTRAINT `FK_PLAYER_BUDDY_BUDDY_TO_PLAYER` FOREIGN KEY (`buddy_identifier`) REFERENCES `player` (`identifier`) ON DELETE CASCADE,
  CONSTRAINT `FK_PLAYER_BUDDY_PLAYER_TO_PLAYER` FOREIGN KEY (`player_identifier`) REFERENCES `player` (`identifier`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `player_card` */

DROP TABLE IF EXISTS `player_card`;

CREATE TABLE `player_card` (
  `player_identifier` int(10) NOT NULL,
  `card_identifier` int(10) NOT NULL,
  `level` tinyint(3) unsigned NOT NULL,
  KEY `INDEX_PLAYER_IDENTIFIER` (`player_identifier`),
  CONSTRAINT `FK_PLAYER_CARD_PLAYER_TO_PLAYER` FOREIGN KEY (`player_identifier`) REFERENCES `player` (`identifier`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `player_item` */

DROP TABLE IF EXISTS `player_item`;

CREATE TABLE `player_item` (
  `player_identifier` int(10) NOT NULL,
  `inventory_type` tinyint(3) unsigned NOT NULL,
  `inventory_slot` smallint(6) NOT NULL,
  `item_identifier` int(10) NOT NULL,
  `unused_scroll_slots` tinyint(3) unsigned NOT NULL DEFAULT '7',
  `used_scroll_slots` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `strength` smallint(5) unsigned NOT NULL DEFAULT '0',
  `dexterity` smallint(5) unsigned NOT NULL DEFAULT '0',
  `intellect` smallint(5) unsigned NOT NULL DEFAULT '0',
  `luck` smallint(5) unsigned NOT NULL DEFAULT '0',
  `health` smallint(5) unsigned NOT NULL DEFAULT '0',
  `mana` smallint(5) unsigned NOT NULL DEFAULT '0',
  `weapon_attack` smallint(5) unsigned NOT NULL DEFAULT '0',
  `magic_attack` smallint(5) unsigned NOT NULL DEFAULT '0',
  `weapon_defense` smallint(5) unsigned NOT NULL DEFAULT '0',
  `magic_defense` smallint(5) unsigned NOT NULL DEFAULT '0',
  `accuracy` smallint(5) unsigned NOT NULL DEFAULT '0',
  `avoidance` smallint(5) unsigned NOT NULL DEFAULT '0',
  `hands` smallint(5) unsigned NOT NULL DEFAULT '0',
  `speed` smallint(5) unsigned NOT NULL DEFAULT '0',
  `jump` smallint(5) unsigned NOT NULL DEFAULT '0',
  `quantity` smallint(5) unsigned NOT NULL DEFAULT '1',
  `owner` varchar(12) NOT NULL DEFAULT '',
  `flags` smallint(5) unsigned NOT NULL DEFAULT '0',
  KEY `INDEX_PLAYER_IDENTIFIER` (`player_identifier`),
  CONSTRAINT `FK_PLAYER_ITEM_PLAYER_TO_PLAYER` FOREIGN KEY (`player_identifier`) REFERENCES `player` (`identifier`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `player_keymap` */

DROP TABLE IF EXISTS `player_keymap`;

CREATE TABLE `player_keymap` (
  `player_identifier` int(10) NOT NULL,
  `type_0` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_0` int(10) unsigned NOT NULL DEFAULT '0',
  `type_1` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_1` int(10) unsigned NOT NULL DEFAULT '0',
  `type_2` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_2` int(10) unsigned NOT NULL DEFAULT '10',
  `type_3` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_3` int(10) unsigned NOT NULL DEFAULT '12',
  `type_4` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_4` int(10) unsigned NOT NULL DEFAULT '13',
  `type_5` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_5` int(10) unsigned NOT NULL DEFAULT '18',
  `type_6` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_6` int(10) unsigned NOT NULL DEFAULT '24',
  `type_7` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_7` int(10) unsigned NOT NULL DEFAULT '21',
  `type_8` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_8` int(10) unsigned NOT NULL DEFAULT '0',
  `type_9` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_9` int(10) unsigned NOT NULL DEFAULT '0',
  `type_10` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_10` int(10) unsigned NOT NULL DEFAULT '0',
  `type_11` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_11` int(10) unsigned NOT NULL DEFAULT '0',
  `type_12` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_12` int(10) unsigned NOT NULL DEFAULT '0',
  `type_13` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_13` int(10) unsigned NOT NULL DEFAULT '0',
  `type_14` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_14` int(10) unsigned NOT NULL DEFAULT '0',
  `type_15` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_15` int(10) unsigned NOT NULL DEFAULT '0',
  `type_16` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_16` int(10) unsigned NOT NULL DEFAULT '8',
  `type_17` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_17` int(10) unsigned NOT NULL DEFAULT '5',
  `type_18` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_18` int(10) unsigned NOT NULL DEFAULT '0',
  `type_19` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_19` int(10) unsigned NOT NULL DEFAULT '4',
  `type_20` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_20` int(10) unsigned NOT NULL DEFAULT '0',
  `type_21` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_21` int(10) unsigned NOT NULL DEFAULT '0',
  `type_22` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_22` int(10) unsigned NOT NULL DEFAULT '0',
  `type_23` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_23` int(10) unsigned NOT NULL DEFAULT '1',
  `type_24` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_24` int(10) unsigned NOT NULL DEFAULT '0',
  `type_25` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_25` int(10) unsigned NOT NULL DEFAULT '19',
  `type_26` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_26` int(10) unsigned NOT NULL DEFAULT '14',
  `type_27` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_27` int(10) unsigned NOT NULL DEFAULT '15',
  `type_28` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_28` int(10) unsigned NOT NULL DEFAULT '0',
  `type_29` tinyint(3) unsigned NOT NULL DEFAULT '5',
  `action_29` int(10) unsigned NOT NULL DEFAULT '52',
  `type_30` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_30` int(10) unsigned NOT NULL DEFAULT '0',
  `type_31` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_31` int(10) unsigned NOT NULL DEFAULT '2',
  `type_32` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_32` int(10) unsigned NOT NULL DEFAULT '0',
  `type_33` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_33` int(10) unsigned NOT NULL DEFAULT '0',
  `type_34` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_34` int(10) unsigned NOT NULL DEFAULT '17',
  `type_35` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_35` int(10) unsigned NOT NULL DEFAULT '11',
  `type_36` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_36` int(10) unsigned NOT NULL DEFAULT '0',
  `type_37` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_37` int(10) unsigned NOT NULL DEFAULT '3',
  `type_38` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_38` int(10) unsigned NOT NULL DEFAULT '20',
  `type_39` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_39` int(10) unsigned NOT NULL DEFAULT '0',
  `type_40` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_40` int(10) unsigned NOT NULL DEFAULT '16',
  `type_41` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_41` int(10) unsigned NOT NULL DEFAULT '23',
  `type_42` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_42` int(10) unsigned NOT NULL DEFAULT '0',
  `type_43` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_43` int(10) unsigned NOT NULL DEFAULT '9',
  `type_44` tinyint(3) unsigned NOT NULL DEFAULT '5',
  `action_44` int(10) unsigned NOT NULL DEFAULT '50',
  `type_45` tinyint(3) unsigned NOT NULL DEFAULT '5',
  `action_45` int(10) unsigned NOT NULL DEFAULT '51',
  `type_46` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_46` int(10) unsigned NOT NULL DEFAULT '6',
  `type_47` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_47` int(10) unsigned NOT NULL DEFAULT '0',
  `type_48` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_48` int(10) unsigned NOT NULL DEFAULT '22',
  `type_49` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_49` int(10) unsigned NOT NULL DEFAULT '0',
  `type_50` tinyint(3) unsigned NOT NULL DEFAULT '4',
  `action_50` int(10) unsigned NOT NULL DEFAULT '7',
  `type_51` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_51` int(10) unsigned NOT NULL DEFAULT '0',
  `type_52` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_52` int(10) unsigned NOT NULL DEFAULT '0',
  `type_53` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_53` int(10) unsigned NOT NULL DEFAULT '0',
  `type_54` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_54` int(10) unsigned NOT NULL DEFAULT '0',
  `type_55` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_55` int(10) unsigned NOT NULL DEFAULT '0',
  `type_56` tinyint(3) unsigned NOT NULL DEFAULT '5',
  `action_56` int(10) unsigned NOT NULL DEFAULT '53',
  `type_57` tinyint(3) unsigned NOT NULL DEFAULT '5',
  `action_57` int(10) unsigned NOT NULL DEFAULT '54',
  `type_58` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_58` int(10) unsigned NOT NULL DEFAULT '0',
  `type_59` tinyint(3) unsigned NOT NULL DEFAULT '6',
  `action_59` int(10) unsigned NOT NULL DEFAULT '100',
  `type_60` tinyint(3) unsigned NOT NULL DEFAULT '6',
  `action_60` int(10) unsigned NOT NULL DEFAULT '101',
  `type_61` tinyint(3) unsigned NOT NULL DEFAULT '6',
  `action_61` int(10) unsigned NOT NULL DEFAULT '102',
  `type_62` tinyint(3) unsigned NOT NULL DEFAULT '6',
  `action_62` int(10) unsigned NOT NULL DEFAULT '103',
  `type_63` tinyint(3) unsigned NOT NULL DEFAULT '6',
  `action_63` int(10) unsigned NOT NULL DEFAULT '104',
  `type_64` tinyint(3) unsigned NOT NULL DEFAULT '6',
  `action_64` int(10) unsigned NOT NULL DEFAULT '105',
  `type_65` tinyint(3) unsigned NOT NULL DEFAULT '6',
  `action_65` int(10) unsigned NOT NULL DEFAULT '106',
  `type_66` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_66` int(10) unsigned NOT NULL DEFAULT '0',
  `type_67` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_67` int(10) unsigned NOT NULL DEFAULT '0',
  `type_68` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_68` int(10) unsigned NOT NULL DEFAULT '0',
  `type_69` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_69` int(10) unsigned NOT NULL DEFAULT '0',
  `type_70` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_70` int(10) unsigned NOT NULL DEFAULT '0',
  `type_71` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_71` int(10) unsigned NOT NULL DEFAULT '0',
  `type_72` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_72` int(10) unsigned NOT NULL DEFAULT '0',
  `type_73` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_73` int(10) unsigned NOT NULL DEFAULT '0',
  `type_74` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_74` int(10) unsigned NOT NULL DEFAULT '0',
  `type_75` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_75` int(10) unsigned NOT NULL DEFAULT '0',
  `type_76` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_76` int(10) unsigned NOT NULL DEFAULT '0',
  `type_77` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_77` int(10) unsigned NOT NULL DEFAULT '0',
  `type_78` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_78` int(10) unsigned NOT NULL DEFAULT '0',
  `type_79` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_79` int(10) unsigned NOT NULL DEFAULT '0',
  `type_80` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_80` int(10) unsigned NOT NULL DEFAULT '0',
  `type_81` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_81` int(10) unsigned NOT NULL DEFAULT '0',
  `type_82` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_82` int(10) unsigned NOT NULL DEFAULT '0',
  `type_83` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_83` int(10) unsigned NOT NULL DEFAULT '0',
  `type_84` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_84` int(10) unsigned NOT NULL DEFAULT '0',
  `type_85` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_85` int(10) unsigned NOT NULL DEFAULT '0',
  `type_86` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_86` int(10) unsigned NOT NULL DEFAULT '0',
  `type_87` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_87` int(10) unsigned NOT NULL DEFAULT '0',
  `type_88` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_88` int(10) unsigned NOT NULL DEFAULT '0',
  `type_89` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `action_89` int(10) unsigned NOT NULL DEFAULT '0',
  KEY `INDEX_PLAYER_IDENTIFIER` (`player_identifier`),
  CONSTRAINT `FK_PLAYER_KEYMAP_PLAYER_TO_PLAYER` FOREIGN KEY (`player_identifier`) REFERENCES `player` (`identifier`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `player_macro` */

DROP TABLE IF EXISTS `player_macro`;

CREATE TABLE `player_macro` (
  `player_identifier` int(10) NOT NULL,
  `slot` tinyint(3) unsigned NOT NULL,
  `name` varchar(255) NOT NULL,
  `shout` tinyint(1) NOT NULL,
  `first_skill_identifier` int(10) NOT NULL,
  `second_skill_identifier` int(10) NOT NULL,
  `third_skill_identifier` int(10) NOT NULL,
  KEY `INDEX_PLAYER_IDENTIFIER` (`player_identifier`),
  CONSTRAINT `FK_PLAYER_MACRO_PLAYER_TO_PLAYER` FOREIGN KEY (`player_identifier`) REFERENCES `player` (`identifier`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `player_quest` */

DROP TABLE IF EXISTS `player_quest`;

CREATE TABLE `player_quest` (
  `player_identifier` int(10) NOT NULL,
  `quest_identifier` int(10) NOT NULL,
  `mob_identifier` int(10) NOT NULL,
  `mob_kills` smallint(5) unsigned NOT NULL,
  `state` varchar(64) NOT NULL,
  `completed` bigint(20) NOT NULL,
  KEY `INDEX_PLAYER_IDENTIFIER` (`player_identifier`),
  KEY `FK_PLAYER_QUEST_MOB_TO_MOB` (`mob_identifier`),
  CONSTRAINT `FK_PLAYER_QUEST_PLAYER_TO_PLAYER` FOREIGN KEY (`player_identifier`) REFERENCES `player` (`identifier`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `player_skill` */

DROP TABLE IF EXISTS `player_skill`;

CREATE TABLE `player_skill` (
  `player_identifier` int(10) NOT NULL,
  `skill_identifier` int(10) NOT NULL,
  `level` tinyint(3) unsigned NOT NULL,
  `max_level` tinyint(3) unsigned NOT NULL,
  `cooldown` smallint(5) unsigned NOT NULL,
  KEY `INDEX_PLAYER_IDENTIFIER` (`player_identifier`),
  CONSTRAINT `FK_PLAYER_SKILL_PLAYER_TO_PLAYER` FOREIGN KEY (`player_identifier`) REFERENCES `player` (`identifier`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `player_teleport` */

DROP TABLE IF EXISTS `player_teleport`;

CREATE TABLE `player_teleport` (
  `player_identifier` int(10) NOT NULL,
  `map_identifier` int(10) NOT NULL,
  `slot` tinyint(3) unsigned NOT NULL,
  `vip` tinyint(1) NOT NULL,
  KEY `INDEX_PLAYER_IDENTIFIER` (`player_identifier`),
  CONSTRAINT `FK_PLAYER_TELEPORT_PLAYER_TO_PLAYER` FOREIGN KEY (`player_identifier`) REFERENCES `player` (`identifier`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
